using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using ERP.Identity.Infrastructure.Data;
using ERP.Inventory.Infrastructure.Data;
using ERP.Sales.Infrastructure.Data;
using ERP.Identity.Application.Repositories;
using ERP.Identity.Application.Services;
using ERP.Identity.Application.Services.Interfaces;
using ERP.Identity.Infrastructure.Repositories;
using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Application.Services;
using ERP.Inventory.Application.Services.Interfaces;
using ERP.Inventory.Infrastructure.Repositories;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Application.Services;
using ERP.Sales.Application.Services.Interfaces;
using ERP.Sales.Infrastructure.Repositories;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;
using ERP.Shared.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. เพิ่ม Authentication & JWT Middleware
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "Your_Default_Very_Secret_Key_32_Chars";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 2. ปรับปรุง Swagger ให้รองรับ JWT (มีปุ่มใส่ Token)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ERP System API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 3. DbContext Registrations
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<SalesDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<IdentityDbContext>());

// --- 4. Repositories (ลงทะเบียนแบบระบุ Context) ---

// Identity
builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<IRoleRepository>(sp => new RoleRepository(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<IUserRoleRepository>(sp => new UserRoleRepository(sp.GetRequiredService<IdentityDbContext>()));

// Inventory
builder.Services.AddScoped<IProductRepository>(sp => new ProductRepository(sp.GetRequiredService<InventoryDbContext>()));
builder.Services.AddScoped<ICategoryRepository>(sp => new CategoryRepository(sp.GetRequiredService<InventoryDbContext>()));

// Sales
builder.Services.AddScoped<ICustomerRepository>(sp => new CustomerRepository(sp.GetRequiredService<SalesDbContext>()));
builder.Services.AddScoped<IOrderRepository>(sp => new OrderRepository(sp.GetRequiredService<SalesDbContext>()));
builder.Services.AddScoped<IOrderItemRepository>(sp => new OrderItemRepository(sp.GetRequiredService<SalesDbContext>()));

// --- 5. Service Registrations & Unit of Work Integration ---
// เราจะฉีด UnitOfWork ที่ถือ Context ของแต่ละ Module เข้าไปใน Service นั้นๆ โดยตรง

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IIdentityService>(sp =>
{
    var context = sp.GetRequiredService<IdentityDbContext>();
    var uow = new UnitOfWork(context); // สร้าง UoW สำหรับ Identity
    var tokenService = sp.GetRequiredService<ITokenService>();
    var userRepo = sp.GetRequiredService<IUserRepository>();
    return new IdentityService(uow, tokenService);
});

builder.Services.AddScoped<IInventoryService>(sp =>
{
    var context = sp.GetRequiredService<InventoryDbContext>();
    var uow = new UnitOfWork(context); // สร้าง UoW สำหรับ Inventory
    return new InventoryService(uow);
});

builder.Services.AddScoped<ISalesService>(sp =>
{
    var context = sp.GetRequiredService<SalesDbContext>();
    var uow = new UnitOfWork(context);
    var identityService = sp.GetRequiredService<IIdentityService>();
    var inventoryService = sp.GetRequiredService<IInventoryService>();
    return new SalesService(uow, identityService, inventoryService);
});

// --- 6. Pipeline configuration ---
var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
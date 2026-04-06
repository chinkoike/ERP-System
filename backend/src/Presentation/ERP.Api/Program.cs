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
using ERP.Purchasing.Application.Repositories;
using ERP.Purchasing.Application.Services;
using ERP.Purchasing.Application.Services.Interfaces;
using ERP.Purchasing.Infrastructure.Data;
using ERP.Purchasing.Infrastructure.Repositories;
using ERP.Finance.Application.Repositories;
using ERP.Finance.Application.Services;
using ERP.Finance.Application.Services.Interfaces;
using ERP.Finance.Infrastructure.Data;
using ERP.Finance.Infrastructure.Repositories;
using ERP.Report.Application.Services;
using ERP.Report.Application.Services.Interfaces;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;
using ERP.Shared.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

// 2. ตั้งค่า CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173") // พอร์ตของฝั่ง React/Vue
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
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
builder.Services.AddDbContext<PurchasingDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<FinanceDbContext>(opt => opt.UseSqlServer(connectionString));
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

// Purchasing
builder.Services.AddScoped<ISupplierRepository>(sp => new SupplierRepository(sp.GetRequiredService<PurchasingDbContext>()));
builder.Services.AddScoped<IPurchaseOrderRepository>(sp => new PurchaseOrderRepository(sp.GetRequiredService<PurchasingDbContext>()));

// Finance
builder.Services.AddScoped<IInvoiceRepository>(sp => new InvoiceRepository(sp.GetRequiredService<FinanceDbContext>()));
builder.Services.AddScoped<IPaymentRepository>(sp => new PaymentRepository(sp.GetRequiredService<FinanceDbContext>()));
builder.Services.AddScoped<IAccountRepository>(sp => new AccountRepository(sp.GetRequiredService<FinanceDbContext>()));

// --- 5. Service Registrations & Unit of Work Integration ---
// เราจะฉีด UnitOfWork ที่ถือ Context ของแต่ละ Module เข้าไปใน Service นั้นๆ โดยตรง

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IIdentityService>(sp =>
{
    var context = sp.GetRequiredService<IdentityDbContext>();
    var uow = new UnitOfWork(context); // สร้าง UoW สำหรับ Identity
    var tokenService = sp.GetRequiredService<ITokenService>();
    var userRepo = sp.GetRequiredService<IUserRepository>();
    var roleRepo = sp.GetRequiredService<IRoleRepository>();
    var userRoleRepo = sp.GetRequiredService<IUserRoleRepository>();
    return new IdentityService(uow, tokenService, userRepo, roleRepo, userRoleRepo);
});

builder.Services.AddScoped<IInventoryService>(sp =>
{
    var context = sp.GetRequiredService<InventoryDbContext>();
    var uow = new UnitOfWork(context); // สร้าง UoW สำหรับ Inventory
    var productRepo = sp.GetRequiredService<IProductRepository>();
    var categoryRepo = sp.GetRequiredService<ICategoryRepository>();
    return new InventoryService(uow, productRepo, categoryRepo);
});

builder.Services.AddScoped<ISalesService>(sp =>
{
    var context = sp.GetRequiredService<SalesDbContext>();
    var uow = new UnitOfWork(context);
    var identityService = sp.GetRequiredService<IIdentityService>();
    var inventoryService = sp.GetRequiredService<IInventoryService>();
    var customerRepo = sp.GetRequiredService<ICustomerRepository>();
    var orderRepo = sp.GetRequiredService<IOrderRepository>();
    var orderItemRepo = sp.GetRequiredService<IOrderItemRepository>();
    return new SalesService(uow, identityService, inventoryService, customerRepo, orderRepo, orderItemRepo);
});

builder.Services.AddScoped<IPurchasingService>(sp =>
{
    var context = sp.GetRequiredService<PurchasingDbContext>();
    var uow = new UnitOfWork(context);
    var inventoryService = sp.GetRequiredService<IInventoryService>();
    var supplierRepo = sp.GetRequiredService<ISupplierRepository>();
    var purchaseOrderRepo = sp.GetRequiredService<IPurchaseOrderRepository>();
    return new PurchasingService(uow, inventoryService, supplierRepo, purchaseOrderRepo);
});

builder.Services.AddScoped<IFinanceService>(sp =>
{
    var context = sp.GetRequiredService<FinanceDbContext>();
    var uow = new UnitOfWork(context);
    var invoiceRepo = sp.GetRequiredService<IInvoiceRepository>();
    var paymentRepo = sp.GetRequiredService<IPaymentRepository>();
    var accountRepo = sp.GetRequiredService<IAccountRepository>();
    var supplierRepo = sp.GetRequiredService<ISupplierRepository>();
    return new FinanceService(uow, invoiceRepo, paymentRepo, accountRepo, supplierRepo);
});

builder.Services.AddScoped<IReportService>(sp =>
{
    var orderRepo = sp.GetRequiredService<IOrderRepository>();
    var orderItemRepo = sp.GetRequiredService<IOrderItemRepository>();
    var customerRepo = sp.GetRequiredService<ICustomerRepository>();
    var productRepo = sp.GetRequiredService<IProductRepository>();
    var categoryRepo = sp.GetRequiredService<ICategoryRepository>();
    var invoiceRepo = sp.GetRequiredService<IInvoiceRepository>();
    var paymentRepo = sp.GetRequiredService<IPaymentRepository>();
    var accountRepo = sp.GetRequiredService<IAccountRepository>();
    return new ReportService(orderRepo, orderItemRepo, customerRepo, productRepo, categoryRepo, invoiceRepo, paymentRepo, accountRepo);
});

// --- 6. Pipeline configuration ---
var app = builder.Build();
app.UseCors(myAllowSpecificOrigins);
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
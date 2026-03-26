using Microsoft.EntityFrameworkCore;
// เพิ่ม using ของ Infrastructure แต่ละโมดูลเพื่อให้รู้จัก DbContext เฉพาะตัว
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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. จดทะเบียน DbContext แยกตามโมดูล (ใช้ Connection String เดียวกันได้ แต่แยก Context)
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<SalesDbContext>(opt => opt.UseSqlServer(connectionString));

// 2. จดทะเบียน Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 3. จดทะเบียน Unit of Work (สำคัญ: ในระบบ Modular คุณต้องตัดสินใจว่าจะใช้ UoW ตัวไหนคุมตัวไหน)
// เบื้องต้นสามารถใช้ DbContext กลาง หรือแยกตามโมดูลก็ได้ 
// ในที่นี้ถ้าคุณแยก DbContext แล้ว UoW ต้องระบุว่าจะใช้ Context ของใคร
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 4. Identity Repositories (ส่ง IdentityDbContext เข้าไป)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

// 5. Inventory Repositories (ส่ง InventoryDbContext เข้าไป)
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// 6. Sales Repositories (ส่ง SalesDbContext เข้าไป)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

// 7. Service Registrations
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISalesService, SalesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
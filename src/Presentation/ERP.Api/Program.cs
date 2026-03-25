using Microsoft.EntityFrameworkCore;
using ERP.Identity.Infrastructure.Data;
using ERP.Identity.Application.Repositories;
using ERP.Identity.Infrastructure.Repositories;
using ERP.Inventory.Infrastructure.Data;
using ERP.Inventory.Application.Repositories;
using ERP.Inventory.Infrastructure.Repositories;
using ERP.Sales.Infrastructure.Data;
using ERP.Sales.Application.Repositories;
using ERP.Sales.Infrastructure.Repositories;
using ERP.Shared;
using ERP.Shared.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ดึง Connection String จาก User Secrets
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// จดทะเบียน DbContext ของทุกโมดูล (ใน Monolith เราใช้ DB ก้อนเดียวกันได้)
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<SalesDbContext>(opt => opt.UseSqlServer(connectionString));

// Generic repository registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Unit of Work registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Identity repositories
builder.Services.AddScoped<IUserRepository>(sp => new UserRepository(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<IRoleRepository>(sp => new RoleRepository(sp.GetRequiredService<IdentityDbContext>()));
builder.Services.AddScoped<IUserRoleRepository>(sp => new UserRoleRepository(sp.GetRequiredService<IdentityDbContext>()));

// Inventory repositories
builder.Services.AddScoped<IProductRepository>(sp => new ProductRepository(sp.GetRequiredService<InventoryDbContext>()));
builder.Services.AddScoped<ICategoryRepository>(sp => new CategoryRepository(sp.GetRequiredService<InventoryDbContext>()));

// Sales repositories
builder.Services.AddScoped<ICustomerRepository>(sp => new CustomerRepository(sp.GetRequiredService<SalesDbContext>()));
builder.Services.AddScoped<IOrderRepository>(sp => new OrderRepository(sp.GetRequiredService<SalesDbContext>()));
builder.Services.AddScoped<IOrderItemRepository>(sp => new OrderItemRepository(sp.GetRequiredService<SalesDbContext>()));

var app = builder.Build();
// ...
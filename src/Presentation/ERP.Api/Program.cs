using Microsoft.EntityFrameworkCore;
using ERP.Identity.Infrastructure.Data;
using ERP.Inventory.Infrastructure.Data;
using ERP.Sales.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ดึง Connection String จาก User Secrets
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// จดทะเบียน DbContext ของทุกโมดูล (ใน Monolith เราใช้ DB ก้อนเดียวกันได้)
builder.Services.AddDbContext<IdentityDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddDbContext<SalesDbContext>(opt => opt.UseSqlServer(connectionString));

var app = builder.Build();
// ...
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# --- 1. Copy ไฟล์ .sln และ .csproj ทั้งหมดเพื่อทำ Restore ---
COPY ["ERP_System.sln", "./"]

# Shared
COPY ["src/Shared/ERP.Shared/ERP.Shared.csproj", "src/Shared/ERP.Shared/"]
COPY ["src/Shared/ERP.Shared.Infrastructure/ERP.Shared.Infrastructure.csproj", "src/Shared/ERP.Shared.Infrastructure/"]

# Identity Module
COPY ["src/Modules/Identity/ERP.Identity.Domain/ERP.Identity.Domain.csproj", "src/Modules/Identity/ERP.Identity.Domain/"]
COPY ["src/Modules/Identity/ERP.Identity.Application/ERP.Identity.Application.csproj", "src/Modules/Identity/ERP.Identity.Application/"]
COPY ["src/Modules/Identity/ERP.Identity.Infrastructure/ERP.Identity.Infrastructure.csproj", "src/Modules/Identity/ERP.Identity.Infrastructure/"]

# Inventory Module
COPY ["src/Modules/Inventory/ERP.Inventory.Domain/ERP.Inventory.Domain.csproj", "src/Modules/Inventory/ERP.Inventory.Domain/"]
COPY ["src/Modules/Inventory/ERP.Inventory.Application/ERP.Inventory.Application.csproj", "src/Modules/Inventory/ERP.Inventory.Application/"]
COPY ["src/Modules/Inventory/ERP.Inventory.Infrastructure/ERP.Inventory.Infrastructure.csproj", "src/Modules/Inventory/ERP.Inventory.Infrastructure/"]

# Sales Module
COPY ["src/Modules/Sales/ERP.Sales.Domain/ERP.Sales.Domain.csproj", "src/Modules/Sales/ERP.Sales.Domain/"]
COPY ["src/Modules/Sales/ERP.Sales.Application/ERP.Sales.Application.csproj", "src/Modules/Sales/ERP.Sales.Application/"]
COPY ["src/Modules/Sales/ERP.Sales.Infrastructure/ERP.Sales.Infrastructure.csproj", "src/Modules/Sales/ERP.Sales.Infrastructure/"]

# Purchasing Module
COPY ["src/Modules/Purchasing/ERP.Purchasing.Domain/ERP.Purchasing.Domain.csproj", "src/Modules/Purchasing/ERP.Purchasing.Domain/"]
COPY ["src/Modules/Purchasing/ERP.Purchasing.Application/ERP.Purchasing.Application.csproj", "src/Modules/Purchasing/ERP.Purchasing.Application/"]
COPY ["src/Modules/Purchasing/ERP.Purchasing.Infrastructure/ERP.Purchasing.Infrastructure.csproj", "src/Modules/Purchasing/ERP.Purchasing.Infrastructure/"]

# Finance Module
COPY ["src/Modules/Finance/ERP.Finance.Domain/ERP.Finance.Domain.csproj", "src/Modules/Finance/ERP.Finance.Domain/"]
COPY ["src/Modules/Finance/ERP.Finance.Application/ERP.Finance.Application.csproj", "src/Modules/Finance/ERP.Finance.Application/"]
COPY ["src/Modules/Finance/ERP.Finance.Infrastructure/ERP.Finance.Infrastructure.csproj", "src/Modules/Finance/ERP.Finance.Infrastructure/"]

# Report Module
COPY ["src/Modules/Report/ERP.Report.Domain/ERP.Report.Domain.csproj", "src/Modules/Report/ERP.Report.Domain/"]
COPY ["src/Modules/Report/ERP.Report.Application/ERP.Report.Application.csproj", "src/Modules/Report/ERP.Report.Application/"]
COPY ["src/Modules/Report/ERP.Report.Infrastructure/ERP.Report.Infrastructure.csproj", "src/Modules/Report/ERP.Report.Infrastructure/"]

# API Presentation
COPY ["src/Presentation/ERP.Api/ERP.Api.csproj", "src/Presentation/ERP.Api/"]

# --- 2. รัน Restore ---
RUN dotnet restore "src/Presentation/ERP.Api/ERP.Api.csproj"

# --- 3. Copy ไฟล์โค้ดทั้งหมด (C#) ที่เหลือเข้าไป ---
COPY . .
WORKDIR "/src/src/Presentation/ERP.Api"

# --- 4. Build และ Publish ---
RUN dotnet publish "ERP.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime (สร้าง Image ตัวสุดท้ายที่พร้อมใช้งาน)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# ตั้งค่าให้ใช้ Legacy Timestamp สำหรับ PostgreSQL (แก้ปัญหา Date)
ENV Npgsql_EnableLegacyTimestampBehavior=true
# ตั้งค่า Environment เป็น Production
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80
ENTRYPOINT ["dotnet", "ERP.Api.dll"]
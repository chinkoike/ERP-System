# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copy ไฟล์ .csproj ทั้งหมดเพื่อทำ Restore (ช่วยให้ Build เร็วขึ้นด้วย Cache)
# เลียนแบบโครงสร้างโฟลเดอร์เดิมเพื่อให้หา Reference กันเจอ
COPY ["src/Shared/ERP.Shared.Infrastructure/ERP.Shared.Infrastructure.csproj", "src/Shared/ERP.Shared.Infrastructure/"]
COPY ["src/Modules/Identity/ERP.Identity.Application/ERP.Identity.Application.csproj", "src/Modules/Identity/ERP.Identity.Application/"]
COPY ["src/Modules/Identity/ERP.Identity.Infrastructure/ERP.Identity.Infrastructure.csproj", "src/Modules/Identity/ERP.Identity.Infrastructure/"]
COPY ["src/Modules/Sales/ERP.Sales.Application/ERP.Sales.Application.csproj", "src/Modules/Sales/ERP.Sales.Application/"]
COPY ["src/Modules/Sales/ERP.Sales.Infrastructure/ERP.Sales.Infrastructure.csproj", "src/Modules/Sales/ERP.Sales.Infrastructure/"]
COPY ["src/Modules/Inventory/ERP.Inventory.Application/ERP.Inventory.Application.csproj", "src/Modules/Inventory/ERP.Inventory.Application/"]
COPY ["src/Modules/Inventory/ERP.Inventory.Infrastructure/ERP.Inventory.Infrastructure.csproj", "src/Modules/Inventory/ERP.Inventory.Infrastructure/"]
COPY ["src/Modules/Finance/ERP.Finance.Application/ERP.Finance.Application.csproj", "src/Modules/Finance/ERP.Finance.Application/"]
COPY ["src/Modules/Finance/ERP.Finance.Infrastructure/ERP.Finance.Infrastructure.csproj", "src/Modules/Finance/ERP.Finance.Infrastructure/"]
COPY ["src/Modules/Report/ERP.Report.Application/ERP.Report.Application.csproj", "src/Modules/Report/ERP.Report.Application/"]
COPY ["src/Modules/Report/ERP.Report.Infrastructure/ERP.Report.Infrastructure.csproj", "src/Modules/Report/ERP.Report.Infrastructure/"]
COPY ["src/Presentation/ERP.Api/ERP.Api.csproj", "src/Presentation/ERP.Api/"]

# 2. รัน Restore เพื่อโหลด Library
RUN dotnet restore "src/Presentation/ERP.Api/ERP.Api.csproj"

# 3. Copy ไฟล์ที่เหลือทั้งหมดและ Build
COPY . .
WORKDIR "/src/Presentation/ERP.Api"
RUN dotnet build "ERP.Api.csproj" -c Release -o /app/build

# 4. Publish เป็นไฟล์พร้อมรัน
FROM build AS publish
RUN dotnet publish "ERP.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime (ขนาดเล็ก)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# ตั้งค่าให้ใช้ Legacy Timestamp สำหรับ PostgreSQL
ENV Npgsql_EnableLegacyTimestampBehavior=true

EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "ERP.Api.dll"]
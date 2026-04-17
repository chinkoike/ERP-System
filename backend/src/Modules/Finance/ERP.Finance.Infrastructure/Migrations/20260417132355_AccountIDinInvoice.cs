using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Finance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AccountIDinInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Invoices",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Invoices");
        }
    }
}

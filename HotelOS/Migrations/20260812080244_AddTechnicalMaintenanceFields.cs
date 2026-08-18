using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelOS.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalMaintenanceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MaintenanceDate",
                table: "TechnicalEquipments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceDescription",
                table: "TechnicalEquipments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceEmployeeId",
                table: "TechnicalEquipments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceDate",
                table: "TechnicalEquipments");

            migrationBuilder.DropColumn(
                name: "MaintenanceDescription",
                table: "TechnicalEquipments");

            migrationBuilder.DropColumn(
                name: "MaintenanceEmployeeId",
                table: "TechnicalEquipments");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelOS.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalEquipmentMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "TechnicalEquipmentId",
                table: "MaintenanceRequests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_TechnicalEquipmentId",
                table: "MaintenanceRequests",
                column: "TechnicalEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_TechnicalEquipments_TechnicalEquipmentId",
                table: "MaintenanceRequests",
                column: "TechnicalEquipmentId",
                principalTable: "TechnicalEquipments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_TechnicalEquipments_TechnicalEquipmentId",
                table: "MaintenanceRequests");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_TechnicalEquipmentId",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "TechnicalEquipmentId",
                table: "MaintenanceRequests");

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
    }
}

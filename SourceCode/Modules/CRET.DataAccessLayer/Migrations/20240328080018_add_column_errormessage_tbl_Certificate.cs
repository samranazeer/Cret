using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRET.DataAccessLayer.Migrations
{
    public partial class add_column_errormessage_tbl_Certificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImportCSRError",
                table: "Certificate",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Setting",
                columns: new[] { "Id", "BatchValidity", "ConsumerValidity", "CreatedAt", "CreatedBy", "InsValidity", "LabValidity", "ProductionValidity", "PulseValidity", "ServiceValidity" },
                values: new object[] { new Guid("6c628d2c-9322-469f-861d-436472fe2627"), 90, 730, new DateTime(2024, 3, 28, 15, 40, 41, 0, DateTimeKind.Unspecified), "Admin", 90, 90, 90, 90, 90 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Setting",
                keyColumn: "Id",
                keyValue: new Guid("6c628d2c-9322-469f-861d-436472fe2627"));

            migrationBuilder.DropColumn(
                name: "ImportCSRError",
                table: "Certificate");
        }
    }
}

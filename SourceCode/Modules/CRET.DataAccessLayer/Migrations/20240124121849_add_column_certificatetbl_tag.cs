using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRET.DataAccessLayer.Migrations
{
    public partial class add_column_certificatetbl_tag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Certificate",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Certificate");
        }
    }
}

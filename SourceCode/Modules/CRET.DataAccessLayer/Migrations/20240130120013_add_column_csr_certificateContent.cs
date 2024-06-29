using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRET.DataAccessLayer.Migrations
{
    public partial class add_column_csr_certificateContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CertificateContent",
                table: "Certificate",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CsrContent",
                table: "Certificate",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateContent",
                table: "Certificate");

            migrationBuilder.DropColumn(
                name: "CsrContent",
                table: "Certificate");
        }
    }
}

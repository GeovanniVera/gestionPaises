using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionpaises.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBanderaPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BanderaPath",
                table: "country",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BanderaPath",
                table: "country");
        }
    }
}

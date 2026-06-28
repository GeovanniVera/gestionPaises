using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionpaises.Migrations
{
    /// <inheritdoc />
    public partial class CambiarCascadeARestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_city_country_CountryCode",
                table: "city");

            migrationBuilder.DropForeignKey(
                name: "FK_countrylanguage_country_CountryCode",
                table: "countrylanguage");

            migrationBuilder.AddForeignKey(
                name: "FK_city_country_CountryCode",
                table: "city",
                column: "CountryCode",
                principalTable: "country",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_countrylanguage_country_CountryCode",
                table: "countrylanguage",
                column: "CountryCode",
                principalTable: "country",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_city_country_CountryCode",
                table: "city");

            migrationBuilder.DropForeignKey(
                name: "FK_countrylanguage_country_CountryCode",
                table: "countrylanguage");

            migrationBuilder.AddForeignKey(
                name: "FK_city_country_CountryCode",
                table: "city",
                column: "CountryCode",
                principalTable: "country",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_countrylanguage_country_CountryCode",
                table: "countrylanguage",
                column: "CountryCode",
                principalTable: "country",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

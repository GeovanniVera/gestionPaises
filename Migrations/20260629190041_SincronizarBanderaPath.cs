using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestionpaises.Migrations
{
    /// <inheritdoc />
    public partial class SincronizarBanderaPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Borramos los AlterColumn. 
            // Al dejarlo vacío, MySQL no ejecutará comandos peligrosos,
            // pero EF marcará el modelo como "sincronizado".
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // También lo dejamos vacío.
        }
    }
}
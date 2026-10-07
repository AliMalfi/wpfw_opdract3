using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wpfw_opdracht3.Migrations
{
    /// <inheritdoc />
    public partial class RenameProjectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Projects",
                newName: "Titel");

            migrationBuilder.RenameColumn(
                name: "Technologie",
                table: "Projects",
                newName: "Categorie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Titel",
                table: "Projects",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Categorie",
                table: "Projects",
                newName: "Technologie");
        }
    }
}

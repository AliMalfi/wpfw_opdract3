using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace wpfw_opdracht3.Migrations
{
    /// <inheritdoc />
    public partial class BlogpostUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Datum",
                table: "Blogposts",
                newName: "Publicatiedatum");

            migrationBuilder.InsertData(
                table: "Blogposts",
                columns: new[] { "Id", "Inhoud", "Publicatiedatum", "Titel" },
                values: new object[,]
                {
                    { 1, "Deze week ben ik begonnen met React voor mijn Smart Environment Dashboard.", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mijn eerste stappen met React" },
                    { 2, "Tijdens het bouwen van mijn portfolio leerde ik hoe belangrijk toegankelijkheid is.", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Waarom semantische HTML ertoe doet" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Blogposts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Blogposts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.RenameColumn(
                name: "Publicatiedatum",
                table: "Blogposts",
                newName: "Datum");
        }
    }
}

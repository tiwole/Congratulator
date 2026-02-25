using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Congratulator.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Persons",
                type: "text",
                nullable: true,
                defaultValue: "default.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Persons");
        }
    }
}

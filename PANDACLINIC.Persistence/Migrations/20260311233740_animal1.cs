using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PANDACLINIC.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class animal1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnimalType",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimalType",
                table: "Animals");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSystemPropertyToPropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemProperty",
                table: "PropertyTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemProperty",
                table: "PropertyTypes");
        }
    }
}

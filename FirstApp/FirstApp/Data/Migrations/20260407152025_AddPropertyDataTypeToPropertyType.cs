using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyDataTypeToPropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataType",
                table: "PropertyTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "PropertyTypes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wyrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectTypePluralName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PluralName",
                table: "ObjectTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"ObjectTypes\" SET \"PluralName\" = \"Name\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PluralName",
                table: "ObjectTypes");
        }
    }
}

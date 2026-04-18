using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCascadeDeleteForPropertyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyTypes_ObjectTypes_ObjectTypeId",
                table: "PropertyTypes");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyTypes_ObjectTypes_ObjectTypeId",
                table: "PropertyTypes",
                column: "ObjectTypeId",
                principalTable: "ObjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyTypes_ObjectTypes_ObjectTypeId",
                table: "PropertyTypes");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyTypes_ObjectTypes_ObjectTypeId",
                table: "PropertyTypes",
                column: "ObjectTypeId",
                principalTable: "ObjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

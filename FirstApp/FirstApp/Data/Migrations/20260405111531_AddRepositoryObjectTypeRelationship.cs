using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryObjectTypeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ObjectTypeId",
                table: "Repositories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_ObjectTypeId",
                table: "Repositories",
                column: "ObjectTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Repositories_ObjectTypes_ObjectTypeId",
                table: "Repositories",
                column: "ObjectTypeId",
                principalTable: "ObjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Repositories_ObjectTypes_ObjectTypeId",
                table: "Repositories");

            migrationBuilder.DropIndex(
                name: "IX_Repositories_ObjectTypeId",
                table: "Repositories");

            migrationBuilder.DropColumn(
                name: "ObjectTypeId",
                table: "Repositories");
        }
    }
}

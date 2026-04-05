using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReverseRepositoryObjectTypeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "RepositoryId",
                table: "ObjectTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectTypes_RepositoryId",
                table: "ObjectTypes",
                column: "RepositoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectTypes_Repositories_RepositoryId",
                table: "ObjectTypes",
                column: "RepositoryId",
                principalTable: "Repositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectTypes_Repositories_RepositoryId",
                table: "ObjectTypes");

            migrationBuilder.DropIndex(
                name: "IX_ObjectTypes_RepositoryId",
                table: "ObjectTypes");

            migrationBuilder.DropColumn(
                name: "RepositoryId",
                table: "ObjectTypes");

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
    }
}

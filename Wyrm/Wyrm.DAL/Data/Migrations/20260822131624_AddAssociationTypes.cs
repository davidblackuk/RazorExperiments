using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wyrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssociationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssociationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ForwardName = table.Column<string>(type: "TEXT", nullable: false),
                    ReverseName = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RepositoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceObjectTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetObjectTypeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationTypes_ObjectTypes_SourceObjectTypeId",
                        column: x => x.SourceObjectTypeId,
                        principalTable: "ObjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationTypes_ObjectTypes_TargetObjectTypeId",
                        column: x => x.TargetObjectTypeId,
                        principalTable: "ObjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationTypes_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssociationInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociationTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SourceObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationInstances_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationInstances_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationInstances_AssociationTypes_AssociationTypeId",
                        column: x => x.AssociationTypeId,
                        principalTable: "AssociationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationInstances_ObjectInstances_SourceObjectInstanceId",
                        column: x => x.SourceObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationInstances_ObjectInstances_TargetObjectInstanceId",
                        column: x => x.TargetObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssociationPropertyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsSystemProperty = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AssociationTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationPropertyTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyTypes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssociationPropertyTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssociationPropertyTypes_AssociationTypes_AssociationTypeId",
                        column: x => x.AssociationTypeId,
                        principalTable: "AssociationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssociationInstances_AssociationTypeId",
                table: "AssociationInstances",
                column: "AssociationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationInstances_CreatedById",
                table: "AssociationInstances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationInstances_SourceObjectInstanceId",
                table: "AssociationInstances",
                column: "SourceObjectInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationInstances_TargetObjectInstanceId",
                table: "AssociationInstances",
                column: "TargetObjectInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationInstances_UpdatedById",
                table: "AssociationInstances",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyTypes_AssociationTypeId",
                table: "AssociationPropertyTypes",
                column: "AssociationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyTypes_CreatedById",
                table: "AssociationPropertyTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyTypes_UpdatedById",
                table: "AssociationPropertyTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTypes_CreatedById",
                table: "AssociationTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTypes_RepositoryId",
                table: "AssociationTypes",
                column: "RepositoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTypes_SourceObjectTypeId",
                table: "AssociationTypes",
                column: "SourceObjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTypes_TargetObjectTypeId",
                table: "AssociationTypes",
                column: "TargetObjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationTypes_UpdatedById",
                table: "AssociationTypes",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssociationInstances");

            migrationBuilder.DropTable(
                name: "AssociationPropertyTypes");

            migrationBuilder.DropTable(
                name: "AssociationTypes");
        }
    }
}

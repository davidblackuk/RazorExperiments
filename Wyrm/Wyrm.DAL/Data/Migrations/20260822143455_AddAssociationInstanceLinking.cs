using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wyrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssociationInstanceLinking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_SourceObjectInstanceId",
                table: "AssociationInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_TargetObjectInstanceId",
                table: "AssociationInstances");

            migrationBuilder.CreateTable(
                name: "AssociationPropertyValueDateTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociationInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssociationPropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationPropertyValueDateTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueDateTimes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueDateTimes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueDateTimes_AssociationInstances_AssociationInstanceId",
                        column: x => x.AssociationInstanceId,
                        principalTable: "AssociationInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueDateTimes_AssociationPropertyTypes_AssociationPropertyTypeId",
                        column: x => x.AssociationPropertyTypeId,
                        principalTable: "AssociationPropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationPropertyValueInts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociationInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssociationPropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationPropertyValueInts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueInts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueInts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueInts_AssociationInstances_AssociationInstanceId",
                        column: x => x.AssociationInstanceId,
                        principalTable: "AssociationInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueInts_AssociationPropertyTypes_AssociationPropertyTypeId",
                        column: x => x.AssociationPropertyTypeId,
                        principalTable: "AssociationPropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationPropertyValueNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociationInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssociationPropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationPropertyValueNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueNumbers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueNumbers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueNumbers_AssociationInstances_AssociationInstanceId",
                        column: x => x.AssociationInstanceId,
                        principalTable: "AssociationInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueNumbers_AssociationPropertyTypes_AssociationPropertyTypeId",
                        column: x => x.AssociationPropertyTypeId,
                        principalTable: "AssociationPropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociationPropertyValueStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssociationInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssociationPropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociationPropertyValueStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueStrings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueStrings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueStrings_AssociationInstances_AssociationInstanceId",
                        column: x => x.AssociationInstanceId,
                        principalTable: "AssociationInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociationPropertyValueStrings_AssociationPropertyTypes_AssociationPropertyTypeId",
                        column: x => x.AssociationPropertyTypeId,
                        principalTable: "AssociationPropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueDateTimes_AssociationInstanceId_AssociationPropertyTypeId",
                table: "AssociationPropertyValueDateTimes",
                columns: new[] { "AssociationInstanceId", "AssociationPropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueDateTimes_AssociationPropertyTypeId",
                table: "AssociationPropertyValueDateTimes",
                column: "AssociationPropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueDateTimes_CreatedById",
                table: "AssociationPropertyValueDateTimes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueDateTimes_UpdatedById",
                table: "AssociationPropertyValueDateTimes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueInts_AssociationInstanceId_AssociationPropertyTypeId",
                table: "AssociationPropertyValueInts",
                columns: new[] { "AssociationInstanceId", "AssociationPropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueInts_AssociationPropertyTypeId",
                table: "AssociationPropertyValueInts",
                column: "AssociationPropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueInts_CreatedById",
                table: "AssociationPropertyValueInts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueInts_UpdatedById",
                table: "AssociationPropertyValueInts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueNumbers_AssociationInstanceId_AssociationPropertyTypeId",
                table: "AssociationPropertyValueNumbers",
                columns: new[] { "AssociationInstanceId", "AssociationPropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueNumbers_AssociationPropertyTypeId",
                table: "AssociationPropertyValueNumbers",
                column: "AssociationPropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueNumbers_CreatedById",
                table: "AssociationPropertyValueNumbers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueNumbers_UpdatedById",
                table: "AssociationPropertyValueNumbers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueStrings_AssociationInstanceId_AssociationPropertyTypeId",
                table: "AssociationPropertyValueStrings",
                columns: new[] { "AssociationInstanceId", "AssociationPropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueStrings_AssociationPropertyTypeId",
                table: "AssociationPropertyValueStrings",
                column: "AssociationPropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueStrings_CreatedById",
                table: "AssociationPropertyValueStrings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AssociationPropertyValueStrings_UpdatedById",
                table: "AssociationPropertyValueStrings",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_SourceObjectInstanceId",
                table: "AssociationInstances",
                column: "SourceObjectInstanceId",
                principalTable: "ObjectInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_TargetObjectInstanceId",
                table: "AssociationInstances",
                column: "TargetObjectInstanceId",
                principalTable: "ObjectInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_SourceObjectInstanceId",
                table: "AssociationInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_TargetObjectInstanceId",
                table: "AssociationInstances");

            migrationBuilder.DropTable(
                name: "AssociationPropertyValueDateTimes");

            migrationBuilder.DropTable(
                name: "AssociationPropertyValueInts");

            migrationBuilder.DropTable(
                name: "AssociationPropertyValueNumbers");

            migrationBuilder.DropTable(
                name: "AssociationPropertyValueStrings");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_SourceObjectInstanceId",
                table: "AssociationInstances",
                column: "SourceObjectInstanceId",
                principalTable: "ObjectInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssociationInstances_ObjectInstances_TargetObjectInstanceId",
                table: "AssociationInstances",
                column: "TargetObjectInstanceId",
                principalTable: "ObjectInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

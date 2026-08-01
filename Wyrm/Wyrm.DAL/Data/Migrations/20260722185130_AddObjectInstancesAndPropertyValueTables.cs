using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wyrm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectInstancesAndPropertyValueTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObjectInstances_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectInstances_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectInstances_ObjectTypes_ObjectTypeId",
                        column: x => x.ObjectTypeId,
                        principalTable: "ObjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyValueDateTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyValueDateTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyValueDateTimes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueDateTimes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueDateTimes_ObjectInstances_ObjectInstanceId",
                        column: x => x.ObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyValueDateTimes_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyValueInts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyValueInts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyValueInts_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueInts_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueInts_ObjectInstances_ObjectInstanceId",
                        column: x => x.ObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyValueInts_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyValueNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyValueNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyValueNumbers_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueNumbers_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueNumbers_ObjectInstances_ObjectInstanceId",
                        column: x => x.ObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyValueNumbers_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyValueStrings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ObjectInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedById = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyValueStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyValueStrings_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueStrings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyValueStrings_ObjectInstances_ObjectInstanceId",
                        column: x => x.ObjectInstanceId,
                        principalTable: "ObjectInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyValueStrings_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectInstances_CreatedById",
                table: "ObjectInstances",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectInstances_ObjectTypeId",
                table: "ObjectInstances",
                column: "ObjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectInstances_UpdatedById",
                table: "ObjectInstances",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueDateTimes_CreatedById",
                table: "PropertyValueDateTimes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueDateTimes_ObjectInstanceId_PropertyTypeId",
                table: "PropertyValueDateTimes",
                columns: new[] { "ObjectInstanceId", "PropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueDateTimes_PropertyTypeId",
                table: "PropertyValueDateTimes",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueDateTimes_UpdatedById",
                table: "PropertyValueDateTimes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueInts_CreatedById",
                table: "PropertyValueInts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueInts_ObjectInstanceId_PropertyTypeId",
                table: "PropertyValueInts",
                columns: new[] { "ObjectInstanceId", "PropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueInts_PropertyTypeId",
                table: "PropertyValueInts",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueInts_UpdatedById",
                table: "PropertyValueInts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueNumbers_CreatedById",
                table: "PropertyValueNumbers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueNumbers_ObjectInstanceId_PropertyTypeId",
                table: "PropertyValueNumbers",
                columns: new[] { "ObjectInstanceId", "PropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueNumbers_PropertyTypeId",
                table: "PropertyValueNumbers",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueNumbers_UpdatedById",
                table: "PropertyValueNumbers",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueStrings_CreatedById",
                table: "PropertyValueStrings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueStrings_ObjectInstanceId_PropertyTypeId",
                table: "PropertyValueStrings",
                columns: new[] { "ObjectInstanceId", "PropertyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueStrings_PropertyTypeId",
                table: "PropertyValueStrings",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyValueStrings_UpdatedById",
                table: "PropertyValueStrings",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyValueDateTimes");

            migrationBuilder.DropTable(
                name: "PropertyValueInts");

            migrationBuilder.DropTable(
                name: "PropertyValueNumbers");

            migrationBuilder.DropTable(
                name: "PropertyValueStrings");

            migrationBuilder.DropTable(
                name: "ObjectInstances");
        }
    }
}

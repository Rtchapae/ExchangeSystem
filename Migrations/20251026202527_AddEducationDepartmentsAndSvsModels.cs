using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationDepartmentsAndSvsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationDepartmentId",
                table: "Stores",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EducationDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DirectorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SvsCatalogUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalMaterials = table.Column<int>(type: "integer", nullable: false),
                    MappedMaterials = table.Column<int>(type: "integer", nullable: false),
                    UnmappedMaterials = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdateSource = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EducationDepartmentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SvsCatalogUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SvsCatalogUpdates_EducationDepartments_EducationDepartmentId",
                        column: x => x.EducationDepartmentId,
                        principalTable: "EducationDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SvsMaterialMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SvsMatId = table.Column<int>(type: "integer", nullable: false),
                    SvsMaterialName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SvsGroupId = table.Column<int>(type: "integer", nullable: false),
                    SvsGroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SvsMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: true),
                    SvsCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsAutoMapped = table.Column<bool>(type: "boolean", nullable: false),
                    Confidence = table.Column<double>(type: "double precision", nullable: false),
                    MappingNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CatalogUpdateId = table.Column<int>(type: "integer", nullable: false),
                    EducationDepartmentId = table.Column<int>(type: "integer", nullable: true),
                    OrganizationId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SvsMaterialMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SvsMaterialMappings_EducationDepartments_EducationDepartmen~",
                        column: x => x.EducationDepartmentId,
                        principalTable: "EducationDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SvsMaterialMappings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SvsMaterialMappings_Stores_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SvsMaterialMappings_SvsCatalogUpdates_CatalogUpdateId",
                        column: x => x.CatalogUpdateId,
                        principalTable: "SvsCatalogUpdates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8000), new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8000) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8000), new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8000) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8010), new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(8010) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(7530), new DateTime(2025, 10, 26, 20, 25, 27, 522, DateTimeKind.Utc).AddTicks(7530), "$2a$11$KQOd2gIMuA60nGxHTcUzoO/hSZX0VfsU34cmzS1RUZRsP4YRTDSe." });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_EducationDepartmentId",
                table: "Stores",
                column: "EducationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SvsCatalogUpdates_EducationDepartmentId",
                table: "SvsCatalogUpdates",
                column: "EducationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SvsMaterialMappings_CatalogUpdateId",
                table: "SvsMaterialMappings",
                column: "CatalogUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_SvsMaterialMappings_EducationDepartmentId",
                table: "SvsMaterialMappings",
                column: "EducationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SvsMaterialMappings_OrganizationId",
                table: "SvsMaterialMappings",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SvsMaterialMappings_ProductId",
                table: "SvsMaterialMappings",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_EducationDepartments_EducationDepartmentId",
                table: "Stores",
                column: "EducationDepartmentId",
                principalTable: "EducationDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_EducationDepartments_EducationDepartmentId",
                table: "Stores");

            migrationBuilder.DropTable(
                name: "SvsMaterialMappings");

            migrationBuilder.DropTable(
                name: "SvsCatalogUpdates");

            migrationBuilder.DropTable(
                name: "EducationDepartments");

            migrationBuilder.DropIndex(
                name: "IX_Stores_EducationDepartmentId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "EducationDepartmentId",
                table: "Stores");

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(710), new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(710) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(720), new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(710) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(720), new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(720) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(30), new DateTime(2025, 10, 19, 23, 56, 38, 775, DateTimeKind.Utc).AddTicks(30), "$2a$11$SgQExkwVNBQiLvl6m6J84OX7UtvQ3TcrSwlaNX6zoZo6rPsKzDFcy" });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganizationProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrganizationId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SvsCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LocalPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProducts_Stores_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProducts_OrganizationId_ProductId",
                table: "OrganizationProducts",
                columns: new[] { "OrganizationId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProducts_ProductId",
                table: "OrganizationProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationProducts");

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250), new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250), new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250), new DateTime(2025, 10, 19, 23, 40, 51, 235, DateTimeKind.Utc).AddTicks(250) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 19, 23, 40, 51, 234, DateTimeKind.Utc).AddTicks(8890), new DateTime(2025, 10, 19, 23, 40, 51, 234, DateTimeKind.Utc).AddTicks(8890), "$2a$11$3amdcSnpNX5byCHJjPJARu.rpzxFVoKvwzhQJoah.ZJf0MnQ7Fsfy" });
        }
    }
}

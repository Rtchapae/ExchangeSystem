using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSvsMaterialsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SvsMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupMatId = table.Column<int>(type: "integer", nullable: false),
                    MatId = table.Column<int>(type: "integer", nullable: false),
                    MeasureId = table.Column<int>(type: "integer", nullable: false),
                    NameGroupMat = table.Column<string>(type: "text", nullable: false),
                    NameMat = table.Column<string>(type: "text", nullable: false),
                    NameMeasure = table.Column<string>(type: "text", nullable: false),
                    EducationDepartmentId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SvsMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SvsMaterials_EducationDepartments_EducationDepartmentId",
                        column: x => x.EducationDepartmentId,
                        principalTable: "EducationDepartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9413), new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9407) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9417), new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9415) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9426), new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(9425) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(6809), new DateTime(2025, 10, 27, 0, 0, 53, 289, DateTimeKind.Utc).AddTicks(6834), "$2a$11$uzAqy.C1zd08y/XuteeyBeZBP6dSQzP.XnqgstnRwWIDcEj5Y/aIu" });

            migrationBuilder.CreateIndex(
                name: "IX_SvsMaterials_EducationDepartmentId",
                table: "SvsMaterials",
                column: "EducationDepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SvsMaterials");

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
        }
    }
}

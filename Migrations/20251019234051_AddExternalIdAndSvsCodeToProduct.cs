using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalIdAndSvsCodeToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SvsCode",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SvsCode",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1070) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(280), new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(280), "$2a$11$Ot50S3vLLziqlrxYjGXcKeQ3h6.2Hmqjqkk2RGW/TGnc29YAjZy8." });
        }
    }
}

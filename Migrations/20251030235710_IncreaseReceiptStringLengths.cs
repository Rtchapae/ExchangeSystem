using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseReceiptStringLengths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "ProductReceipts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "ProductReceipts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1711), new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1702) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1716), new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1714) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1722), new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(1719) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(362), new DateTime(2025, 10, 30, 23, 57, 9, 939, DateTimeKind.Utc).AddTicks(371), "$2a$11$7q.MawpzxRtQWdx/jAORju0C2n8o3NvHaoXdPpShtc92j0wP6ZASu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentNumber",
                table: "ProductReceipts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ContractNumber",
                table: "ProductReceipts",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6124), new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6121) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6127), new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6126) });

            migrationBuilder.UpdateData(
                table: "ConsumptionCategories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6129), new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(6128) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(5052), new DateTime(2025, 10, 30, 22, 57, 45, 485, DateTimeKind.Utc).AddTicks(5064), "$2a$11$SiKyi8cq8pqibFlUXlrIduPTub37F/SIKbJ0dg/zRsr2qZBbv1sM6" });
        }
    }
}

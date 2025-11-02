using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationFieldsToConsumptionAndReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationDepartmentId",
                table: "ProductReceipts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "ProductReceipts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationDepartmentId",
                table: "ProductConsumptions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "ProductConsumptions",
                type: "integer",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ProductReceipts_OrganizationId",
                table: "ProductReceipts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConsumptions_OrganizationId",
                table: "ProductConsumptions",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConsumptions_EducationDepartmentId",
                table: "ProductConsumptions",
                column: "EducationDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReceipts_EducationDepartmentId",
                table: "ProductReceipts",
                column: "EducationDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConsumptions_Stores_OrganizationId",
                table: "ProductConsumptions",
                column: "OrganizationId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReceipts_Stores_OrganizationId",
                table: "ProductReceipts",
                column: "OrganizationId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductConsumptions_EducationDepartments_EducationDepartmentId",
                table: "ProductConsumptions",
                column: "EducationDepartmentId",
                principalTable: "EducationDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReceipts_EducationDepartments_EducationDepartmentId",
                table: "ProductReceipts",
                column: "EducationDepartmentId",
                principalTable: "EducationDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductConsumptions_EducationDepartments_EducationDepartmentId",
                table: "ProductConsumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductConsumptions_Stores_OrganizationId",
                table: "ProductConsumptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReceipts_EducationDepartments_EducationDepartmentId",
                table: "ProductReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReceipts_Stores_OrganizationId",
                table: "ProductReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ProductReceipts_OrganizationId",
                table: "ProductReceipts");

            migrationBuilder.DropIndex(
                name: "IX_ProductConsumptions_OrganizationId",
                table: "ProductConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductConsumptions_EducationDepartmentId",
                table: "ProductConsumptions");

            migrationBuilder.DropIndex(
                name: "IX_ProductReceipts_EducationDepartmentId",
                table: "ProductReceipts");

            migrationBuilder.DropColumn(
                name: "EducationDepartmentId",
                table: "ProductReceipts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ProductReceipts");

            migrationBuilder.DropColumn(
                name: "EducationDepartmentId",
                table: "ProductConsumptions");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "ProductConsumptions");

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
        }
    }
}

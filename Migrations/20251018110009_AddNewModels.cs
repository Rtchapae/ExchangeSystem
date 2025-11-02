using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExchangeSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumptionCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumptionCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataImportLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImportType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ImportDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalRecords = table.Column<int>(type: "integer", nullable: false),
                    ProcessedRecords = table.Column<int>(type: "integer", nullable: false),
                    SuccessRecords = table.Column<int>(type: "integer", nullable: false),
                    ErrorRecords = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataImportLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExternalProductCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ExternalGroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SvsProductId = table.Column<int>(type: "integer", nullable: true),
                    SvsProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SvsUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SvsGroup = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsAutoMapped = table.Column<bool>(type: "boolean", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductMappings_Products_SvsProductId",
                        column: x => x.SvsProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceiptDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SupplierName = table.Column<string>(type: "character varying(140)", maxLength: 140, nullable: false),
                    SupplierUnp = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    ContractDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,3)", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReceipts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductConsumptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsumptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(15,3)", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    CategoryName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NurseryQuantity = table.Column<decimal>(type: "numeric(15,3)", nullable: true),
                    NurseryCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    KindergartenQuantity = table.Column<decimal>(type: "numeric(15,3)", nullable: true),
                    KindergartenCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    StaffQuantity = table.Column<decimal>(type: "numeric(15,3)", nullable: true),
                    StaffCost = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductConsumptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductConsumptions_ConsumptionCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ConsumptionCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductConsumptions_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DataImportErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImportLogId = table.Column<int>(type: "integer", nullable: false),
                    RowNumber = table.Column<int>(type: "integer", nullable: false),
                    ErrorType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    RowData = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataImportErrors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataImportErrors_DataImportLogs_ImportLogId",
                        column: x => x.ImportLogId,
                        principalTable: "DataImportLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ConsumptionCategories",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "NURSERY", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), "Дети ясельного возраста", true, "Ясли 10,5", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1070) },
                    { 2, "KINDERGARTEN", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), "Дети дошкольного возраста", true, "САД 10,5", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080) },
                    { 3, "STAFF", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080), "Персонал учреждения", true, "Сотрудники", new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(1080) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(280), new DateTime(2025, 10, 18, 11, 0, 8, 912, DateTimeKind.Utc).AddTicks(280), "$2a$11$Ot50S3vLLziqlrxYjGXcKeQ3h6.2Hmqjqkk2RGW/TGnc29YAjZy8." });

            migrationBuilder.CreateIndex(
                name: "IX_DataImportErrors_ImportLogId",
                table: "DataImportErrors",
                column: "ImportLogId");

            migrationBuilder.CreateIndex(
                name: "IX_DataImportLogs_ImportDate",
                table: "DataImportLogs",
                column: "ImportDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConsumptions_CategoryId",
                table: "ProductConsumptions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConsumptions_ConsumptionDate",
                table: "ProductConsumptions",
                column: "ConsumptionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ProductConsumptions_ProductId",
                table: "ProductConsumptions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMappings_ExternalProductCode",
                table: "ProductMappings",
                column: "ExternalProductCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProductMappings_SvsProductId",
                table: "ProductMappings",
                column: "SvsProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReceipts_DocumentNumber",
                table: "ProductReceipts",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReceipts_ProductId",
                table: "ProductReceipts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReceipts_ReceiptDate",
                table: "ProductReceipts",
                column: "ReceiptDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataImportErrors");

            migrationBuilder.DropTable(
                name: "ProductConsumptions");

            migrationBuilder.DropTable(
                name: "ProductMappings");

            migrationBuilder.DropTable(
                name: "ProductReceipts");

            migrationBuilder.DropTable(
                name: "DataImportLogs");

            migrationBuilder.DropTable(
                name: "ConsumptionCategories");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastLoginAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 5, 20, 44, 32, 314, DateTimeKind.Utc).AddTicks(830), new DateTime(2025, 9, 5, 20, 44, 32, 314, DateTimeKind.Utc).AddTicks(830), "$2a$11$SWL1sbN2PR60DND5RiuNbePvjAVv9yMo8kmSpFZzMdUvEZMEDjJZC" });
        }
    }
}

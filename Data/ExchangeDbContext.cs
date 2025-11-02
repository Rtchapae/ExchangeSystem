using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Models;

namespace ExchangeSystem.Data
{
    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext(DbContextOptions<ExchangeDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ProductMapping> ProductMappings { get; set; }
        public DbSet<ProductConsumption> ProductConsumptions { get; set; }
        public DbSet<ConsumptionCategory> ConsumptionCategories { get; set; }
        public DbSet<ProductReceipt> ProductReceipts { get; set; }
        public DbSet<DataImportLog> DataImportLogs { get; set; }
        public DbSet<DataImportError> DataImportErrors { get; set; }
        public DbSet<OrganizationProduct> OrganizationProducts { get; set; }
        public DbSet<EducationDepartment> EducationDepartments { get; set; }
        public DbSet<SvsCatalogUpdate> SvsCatalogUpdates { get; set; }
        public DbSet<SvsMaterialMapping> SvsMaterialMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Product)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Store)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductMapping relationships
            modelBuilder.Entity<ProductMapping>()
                .HasOne(pm => pm.SvsProduct)
                .WithMany()
                .HasForeignKey(pm => pm.SvsProductId)
                .OnDelete(DeleteBehavior.SetNull);

            // ProductConsumption relationships
            modelBuilder.Entity<ProductConsumption>()
                .HasOne(pc => pc.Product)
                .WithMany()
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductConsumption>()
                .HasOne(pc => pc.Category)
                .WithMany()
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductReceipt relationships
            modelBuilder.Entity<ProductReceipt>()
                .HasOne(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // DataImportError relationships
            modelBuilder.Entity<DataImportError>()
                .HasOne(die => die.ImportLog)
                .WithMany(dil => dil.Errors)
                .HasForeignKey(die => die.ImportLogId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrganizationProduct relationships
            modelBuilder.Entity<OrganizationProduct>()
                .HasOne(op => op.Organization)
                .WithMany()
                .HasForeignKey(op => op.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrganizationProduct>()
                .HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: одна комбинация организация+продукт
            modelBuilder.Entity<OrganizationProduct>()
                .HasIndex(op => new { op.OrganizationId, op.ProductId })
                .IsUnique();

            // EducationDepartment relationships
            modelBuilder.Entity<Store>()
                .HasOne(s => s.EducationDepartment)
                .WithMany(ed => ed.Organizations)
                .HasForeignKey(s => s.EducationDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // SvsCatalogUpdate relationships
            modelBuilder.Entity<SvsCatalogUpdate>()
                .HasOne(scu => scu.EducationDepartment)
                .WithMany()
                .HasForeignKey(scu => scu.EducationDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // SvsMaterialMapping relationships
            modelBuilder.Entity<SvsMaterialMapping>()
                .HasOne(smm => smm.CatalogUpdate)
                .WithMany(scu => scu.MaterialMappings)
                .HasForeignKey(smm => smm.CatalogUpdateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SvsMaterialMapping>()
                .HasOne(smm => smm.Product)
                .WithMany()
                .HasForeignKey(smm => smm.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SvsMaterialMapping>()
                .HasOne(smm => smm.EducationDepartment)
                .WithMany()
                .HasForeignKey(smm => smm.EducationDepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SvsMaterialMapping>()
                .HasOne(smm => smm.Organization)
                .WithMany()
                .HasForeignKey(smm => smm.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure indexes
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Code)
                .IsUnique()
                .HasFilter("\"Code\" IS NOT NULL");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Additional indexes for new models
            modelBuilder.Entity<ProductMapping>()
                .HasIndex(pm => pm.ExternalProductCode);

            modelBuilder.Entity<ProductConsumption>()
                .HasIndex(pc => pc.ConsumptionDate);

            modelBuilder.Entity<ProductConsumption>()
                .HasIndex(pc => pc.ProductId);

            modelBuilder.Entity<ProductReceipt>()
                .HasIndex(pr => pr.ReceiptDate);

            modelBuilder.Entity<ProductReceipt>()
                .HasIndex(pr => pr.DocumentNumber);

            modelBuilder.Entity<DataImportLog>()
                .HasIndex(dil => dil.ImportDate);

            // Configure decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Quantity)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Price)
                .HasPrecision(18, 2);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@exchangesystem.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FullName = "Администратор системы",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                }
            );

            // Seed consumption categories
            modelBuilder.Entity<ConsumptionCategory>().HasData(
                new ConsumptionCategory
                {
                    Id = 1,
                    Name = "Ясли 10,5",
                    Code = "NURSERY",
                    Description = "Дети ясельного возраста",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ConsumptionCategory
                {
                    Id = 2,
                    Name = "САД 10,5",
                    Code = "KINDERGARTEN",
                    Description = "Дети дошкольного возраста",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new ConsumptionCategory
                {
                    Id = 3,
                    Name = "Сотрудники",
                    Code = "STAFF",
                    Description = "Персонал учреждения",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}



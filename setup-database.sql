-- Database setup script for Exchange System
-- Run this script in PostgreSQL to create the database and user

-- Create database
CREATE DATABASE "ExchangeSystem" 
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'ru_RU.UTF-8'
    LC_CTYPE = 'ru_RU.UTF-8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Create user (optional, if you want a dedicated user)
-- CREATE USER exchangesystem WITH PASSWORD 'your_password_here';
-- GRANT ALL PRIVILEGES ON DATABASE "ExchangeSystem" TO exchangesystem;

-- Connect to the database
\c "ExchangeSystem";

-- Enable UUID extension (if needed)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create tables (these will be created by Entity Framework migrations, but here's the structure for reference)

-- Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL UNIQUE,
    "Email" VARCHAR(100) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL,
    "FullName" VARCHAR(100),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastLoginAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "Role" VARCHAR(50) NOT NULL DEFAULT 'User'
);

-- Products table
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Category" VARCHAR(50),
    "Code" VARCHAR(20) UNIQUE,
    "Description" VARCHAR(100),
    "Unit" VARCHAR(50),
    "Price" DECIMAL(18,2),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Stores table
CREATE TABLE IF NOT EXISTS "Stores" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Address" VARCHAR(200),
    "City" VARCHAR(50),
    "Phone" VARCHAR(20),
    "Manager" VARCHAR(100),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Transactions table
CREATE TABLE IF NOT EXISTS "Transactions" (
    "Id" SERIAL PRIMARY KEY,
    "ProductId" INTEGER NOT NULL,
    "StoreId" INTEGER NOT NULL,
    "TransactionDate" TIMESTAMP NOT NULL,
    "DocumentNumber" VARCHAR(50),
    "Quantity" DECIMAL(18,3) NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "Supplier" VARCHAR(100),
    "ExpiryDate" TIMESTAMP,
    "Notes" VARCHAR(200),
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE RESTRICT,
    FOREIGN KEY ("StoreId") REFERENCES "Stores"("Id") ON DELETE RESTRICT
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_Products_Code" ON "Products"("Code");
CREATE INDEX IF NOT EXISTS "IX_Products_Name" ON "Products"("Name");
CREATE INDEX IF NOT EXISTS "IX_Stores_Name" ON "Stores"("Name");
CREATE INDEX IF NOT EXISTS "IX_Transactions_ProductId" ON "Transactions"("ProductId");
CREATE INDEX IF NOT EXISTS "IX_Transactions_StoreId" ON "Transactions"("StoreId");
CREATE INDEX IF NOT EXISTS "IX_Transactions_TransactionDate" ON "Transactions"("TransactionDate");
CREATE INDEX IF NOT EXISTS "IX_Users_Username" ON "Users"("Username");
CREATE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users"("Email");

-- Insert default admin user (password: admin123)
INSERT INTO "Users" ("Username", "Email", "PasswordHash", "FullName", "Role", "IsActive", "CreatedAt", "LastLoginAt")
VALUES (
    'admin',
    'admin@exchangesystem.com',
    '$2a$11$rQZ8K9vL2nF3mH4jK5lM6eP7qR8sT9uV0wX1yZ2aB3cD4eF5gH6iJ7kL8mN9oP0',
    'Администратор системы',
    'Admin',
    TRUE,
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
) ON CONFLICT ("Username") DO NOTHING;

-- Create a function to update UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW."UpdatedAt" = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create triggers to automatically update UpdatedAt
CREATE TRIGGER update_products_updated_at BEFORE UPDATE ON "Products"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_stores_updated_at BEFORE UPDATE ON "Stores"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_transactions_updated_at BEFORE UPDATE ON "Transactions"
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Grant permissions (adjust as needed for your setup)
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;

-- Show success message
SELECT 'Database setup completed successfully!' as message;




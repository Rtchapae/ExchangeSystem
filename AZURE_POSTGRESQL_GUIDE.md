# Azure PostgreSQL Database Access Guide

## 🔗 How to Connect to Azure PostgreSQL Database

### 1. **Azure Portal Access**
- Go to [Azure Portal](https://portal.azure.com)
- Navigate to **Azure Database for PostgreSQL** service
- Select your PostgreSQL server instance
- Click on **Connection strings** in the left menu

### 2. **Connection Methods**

#### **Method 1: Azure Portal Query Editor**
- In Azure Portal, go to your PostgreSQL server
- Click on **Query editor** in the left menu
- Login with your admin credentials
- Execute SQL queries directly in the browser

#### **Method 2: psql Command Line**
```bash
# Install PostgreSQL client tools
brew install postgresql@14

# Connect using connection string from Azure Portal
psql "host=your-server.postgres.database.azure.com port=5432 dbname=your-database user=your-username password=your-password sslmode=require"

# Example:
psql "host=exchangesystem.postgres.database.azure.com port=5432 dbname=ExchangeSystem user=admin@exchangesystem password=YourPassword123! sslmode=require"
```

#### **Method 3: pgAdmin (GUI Tool)**
- Download and install [pgAdmin](https://www.pgadmin.org/)
- Create new server connection:
  - **Host**: `your-server.postgres.database.azure.com`
  - **Port**: `5432`
  - **Database**: `your-database-name`
  - **Username**: `your-username@your-server`
  - **Password**: `your-password`
  - **SSL Mode**: `Require`

#### **Method 4: Azure Data Studio**
- Download [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/)
- Install PostgreSQL extension
- Connect using connection string

### 3. **Connection String Format**
```
Server=your-server.postgres.database.azure.com;Port=5432;Database=your-database;User Id=your-username@your-server;Password=your-password;Ssl Mode=Require;
```

### 4. **Common Azure PostgreSQL Commands**

#### **Check Database Status**
```sql
-- List all databases
\l

-- Connect to specific database
\c your-database-name

-- List all tables
\dt

-- Describe table structure
\d "TableName"

-- Check current user
SELECT current_user;

-- Check database version
SELECT version();
```

#### **View Data**
```sql
-- View all products
SELECT * FROM "Products" WHERE "IsActive" = true;

-- View all stores
SELECT * FROM "Stores" WHERE "IsActive" = true;

-- View all transactions
SELECT * FROM "Transactions" ORDER BY "TransactionDate" DESC;

-- View joined data
SELECT 
    t."Id",
    p."Name" as "ProductName",
    s."Name" as "StoreName",
    t."TransactionDate",
    t."Quantity",
    t."Price"
FROM "Transactions" t
INNER JOIN "Products" p ON t."ProductId" = p."Id"
INNER JOIN "Stores" s ON t."StoreId" = s."Id"
ORDER BY t."TransactionDate" DESC;
```

### 5. **Security Best Practices**

#### **Firewall Rules**
- In Azure Portal, go to **Connection security**
- Add your IP address to firewall rules
- Enable **Allow access to Azure services**

#### **SSL Connection**
- Always use SSL connections (`sslmode=require`)
- Download SSL certificate if needed

#### **Connection Pooling**
- Use connection pooling for production
- Set appropriate connection limits

### 6. **Monitoring and Performance**

#### **Azure Metrics**
- Monitor CPU, Memory, Storage usage
- Set up alerts for performance issues
- Check connection count and query performance

#### **Query Performance**
```sql
-- Check slow queries
SELECT query, mean_time, calls 
FROM pg_stat_statements 
ORDER BY mean_time DESC 
LIMIT 10;

-- Check table sizes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables 
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

### 7. **Backup and Restore**

#### **Automated Backups**
- Azure provides automated backups
- Configure backup retention period
- Point-in-time restore available

#### **Manual Backup**
```bash
# Create backup
pg_dump "host=your-server.postgres.database.azure.com port=5432 dbname=your-database user=your-username password=your-password sslmode=require" > backup.sql

# Restore backup
psql "host=your-server.postgres.database.azure.com port=5432 dbname=your-database user=your-username password=your-password sslmode=require" < backup.sql
```

### 8. **Troubleshooting**

#### **Connection Issues**
- Check firewall rules
- Verify SSL settings
- Confirm credentials
- Check server status in Azure Portal

#### **Performance Issues**
- Monitor resource usage
- Check for long-running queries
- Optimize database indexes
- Consider scaling up/down

### 9. **Application Configuration**

#### **Connection String in appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server.postgres.database.azure.com;Port=5432;Database=ExchangeSystem;User Id=your-username@your-server;Password=your-password;Ssl Mode=Require;"
  }
}
```

#### **Environment Variables**
```bash
export ConnectionStrings__DefaultConnection="Server=your-server.postgres.database.azure.com;Port=5432;Database=ExchangeSystem;User Id=your-username@your-server;Password=your-password;Ssl Mode=Require;"
```

### 10. **Useful Azure CLI Commands**

```bash
# List PostgreSQL servers
az postgres server list

# Get connection string
az postgres server show-connection-string --server-name your-server --admin-user your-username --database-name your-database

# Create firewall rule
az postgres server firewall-rule create --resource-group your-resource-group --server your-server --name AllowMyIP --start-ip-address YOUR_IP --end-ip-address YOUR_IP
```

## 🚀 Quick Start Commands

```bash
# 1. Connect to Azure PostgreSQL
psql "host=your-server.postgres.database.azure.com port=5432 dbname=ExchangeSystem user=your-username@your-server password=your-password sslmode=require"

# 2. Check data
SELECT COUNT(*) FROM "Products";
SELECT COUNT(*) FROM "Stores";
SELECT COUNT(*) FROM "Transactions";

# 3. View recent transactions
SELECT p."Name", s."Name", t."TransactionDate", t."Quantity", t."Price"
FROM "Transactions" t
JOIN "Products" p ON t."ProductId" = p."Id"
JOIN "Stores" s ON t."StoreId" = s."Id"
ORDER BY t."TransactionDate" DESC
LIMIT 10;
```

## 📞 Support

- **Azure Support**: [Azure Support Center](https://azure.microsoft.com/en-us/support/)
- **PostgreSQL Documentation**: [PostgreSQL Docs](https://www.postgresql.org/docs/)
- **Azure PostgreSQL Docs**: [Azure PostgreSQL Documentation](https://docs.microsoft.com/en-us/azure/postgresql/)




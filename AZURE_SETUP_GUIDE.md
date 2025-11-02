# Azure PostgreSQL Setup Guide

## 🚀 Quick Setup for Azure PostgreSQL

### 1. Create Azure PostgreSQL Database

1. **Go to Azure Portal**: https://portal.azure.com
2. **Create Resource** → **Databases** → **Azure Database for PostgreSQL**
3. **Choose Flexible Server** (recommended)
4. **Configure**:
   - **Server name**: `your-server-name` (must be globally unique)
   - **Admin username**: `your-admin-username`
   - **Password**: `your-strong-password`
   - **Region**: Choose closest to your location
   - **PostgreSQL version**: 14 or 15

### 2. Configure Connection String

Update `/appsettings.Azure.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_SERVER_NAME.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SslMode=Require;"
  }
}
```

**Replace**:
- `YOUR_SERVER_NAME` → Your Azure server name
- `YOUR_USERNAME` → Your admin username
- `YOUR_PASSWORD` → Your admin password

### 3. Run Application with Azure Database

```bash
# Set environment to use Azure configuration
export ASPNETCORE_ENVIRONMENT=Azure

# Run the application
dotnet run --urls "https://localhost:5001;http://localhost:5002"
```

### 4. Database Migration

```bash
# Apply migrations to Azure database
dotnet ef database update
```

### 5. Add Test Data

```bash
# Connect to Azure database and add test data
psql "Host=YOUR_SERVER_NAME.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SslMode=Require;" -f add-test-data.sql
```

## 🔧 Alternative: Use Environment Variables

Instead of modifying config files, you can use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=YOUR_SERVER_NAME.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SslMode=Require;"
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:5001;http://localhost:5002"
```

## 🔍 Verify Connection

### Check Database Connection:
```bash
# Test connection
psql "Host=YOUR_SERVER_NAME.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USERNAME;Password=YOUR_PASSWORD;Port=5432;SslMode=Require;" -c "\dt"
```

### Check Application:
1. Open browser: https://localhost:5001
2. Login with: admin/admin123
3. Check Dashboard loads data correctly

## 🛡️ Security Best Practices

1. **Use Azure Key Vault** for storing connection strings
2. **Enable SSL** (already configured with `SslMode=Require`)
3. **Use Firewall Rules** to restrict access
4. **Regular Backups** (Azure handles this automatically)

## 📊 Monitoring

- **Azure Portal** → Your PostgreSQL server → **Metrics**
- **Query Performance Insights** for slow queries
- **Connection monitoring** for active connections

## 🚨 Troubleshooting

### Connection Issues:
1. **Check Firewall**: Azure Portal → PostgreSQL → Connection security → Add your IP
2. **Verify Credentials**: Double-check username/password
3. **SSL Issues**: Ensure `SslMode=Require` is set

### Performance Issues:
1. **Check Connection Pooling**: Default is usually fine
2. **Monitor Queries**: Use Azure Query Performance Insights
3. **Scale Up**: Consider upgrading to higher tier if needed

## 💰 Cost Optimization

1. **Use Burstable Tier** for development
2. **Auto-pause** when not in use (if available)
3. **Monitor Usage** in Azure Cost Management

## 🔄 Switching Between Local and Azure

### Local Development:
```bash
export ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

### Azure Production:
```bash
export ASPNETCORE_ENVIRONMENT=Azure
dotnet run
```

## 📝 Example Connection Strings

### Local PostgreSQL:
```
Host=localhost;Database=ExchangeSystem;Username=romanukhnalev;Password=your_password;Port=5432;
```

### Azure PostgreSQL:
```
Host=your-server.postgres.database.azure.com;Database=ExchangeSystem;Username=your_admin;Password=your_password;Port=5432;SslMode=Require;
```

## 🎯 Quick Commands

```bash
# Setup Azure environment
export ASPNETCORE_ENVIRONMENT=Azure
export ConnectionStrings__DefaultConnection="Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;"

# Run application
dotnet run --urls "https://localhost:5001;http://localhost:5002"

# Apply migrations
dotnet ef database update

# Add test data
psql "$ConnectionStrings__DefaultConnection" -f add-test-data.sql
```

## ✅ Verification Checklist

- [ ] Azure PostgreSQL server created
- [ ] Connection string updated
- [ ] Firewall rules configured
- [ ] Application connects successfully
- [ ] Database migrations applied
- [ ] Test data loaded
- [ ] Dashboard shows data correctly




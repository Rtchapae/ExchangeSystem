# 🚀 Quick Azure PostgreSQL Setup

## Step 1: Create Azure PostgreSQL Database

1. **Go to Azure Portal**: https://portal.azure.com
2. **Create Resource** → **Databases** → **Azure Database for PostgreSQL**
3. **Choose Flexible Server**
4. **Configure**:
   - **Server name**: `your-server-name` (must be globally unique)
   - **Admin username**: `your-admin-username`
   - **Password**: `your-strong-password`
   - **Region**: Choose closest to your location
   - **PostgreSQL version**: 14 or 15

## Step 2: Configure Firewall

1. **Go to your PostgreSQL server** in Azure Portal
2. **Connection security** → **Add current client IP address**
3. **Save** the firewall rule

## Step 3: Update Configuration

### Option A: Use the Setup Script
```bash
./setup-azure.sh
```

### Option B: Manual Configuration

1. **Edit `appsettings.Azure.json`**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;"
  }
}
```

2. **Replace**:
   - `YOUR_SERVER` → Your Azure server name
   - `YOUR_USER` → Your admin username
   - `YOUR_PASS` → Your admin password

## Step 4: Run Application

```bash
# Set environment to Azure
export ASPNETCORE_ENVIRONMENT=Azure

# Run the application
dotnet run --urls "https://localhost:5001;http://localhost:5002"
```

## Step 5: Setup Database

```bash
# Apply migrations
dotnet ef database update

# Add test data
psql "Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;" -f add-test-data.sql
```

## Step 6: Test Application

1. **Open browser**: https://localhost:5001
2. **Login**: admin/admin123
3. **Check Dashboard** loads data correctly

## 🔧 Environment Variables (Alternative)

Instead of editing config files, you can use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;"
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:5001;http://localhost:5002"
```

## 🚨 Troubleshooting

### Connection Issues:
1. **Check Firewall**: Ensure your IP is added to Azure PostgreSQL firewall
2. **Verify Credentials**: Double-check username/password
3. **SSL Mode**: Ensure `SslMode=Require` is set

### Performance Issues:
1. **Check Azure Metrics**: Monitor connection count and performance
2. **Scale Up**: Consider upgrading to higher tier if needed

## 📊 Verification

### Test Connection:
```bash
psql "Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;" -c "SELECT version();"
```

### Check Tables:
```bash
psql "Host=YOUR_SERVER.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USER;Password=YOUR_PASS;Port=5432;SslMode=Require;" -c "\dt"
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

## ✅ Success Checklist

- [ ] Azure PostgreSQL server created
- [ ] Firewall rules configured
- [ ] Connection string updated
- [ ] Application connects successfully
- [ ] Database migrations applied
- [ ] Test data loaded
- [ ] Dashboard shows data correctly




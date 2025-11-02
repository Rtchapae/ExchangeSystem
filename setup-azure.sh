#!/bin/bash

# Azure PostgreSQL Setup Script for ExchangeSystem
# This script helps you configure the application to use Azure PostgreSQL

echo "🚀 Azure PostgreSQL Setup for ExchangeSystem"
echo "=============================================="

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first:"
    echo "   brew install azure-cli"
    echo "   or visit: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

echo "✅ Azure CLI found"

# Check if user is logged in
if ! az account show &> /dev/null; then
    echo "🔐 Please log in to Azure:"
    az login
fi

echo "✅ Azure login verified"

# Get Azure subscription info
SUBSCRIPTION=$(az account show --query "name" -o tsv)
echo "📋 Current subscription: $SUBSCRIPTION"

echo ""
echo "📝 Please provide the following information:"
echo ""

# Get server name
read -p "Enter your Azure PostgreSQL server name (without .postgres.database.azure.com): " SERVER_NAME
if [ -z "$SERVER_NAME" ]; then
    echo "❌ Server name is required"
    exit 1
fi

# Get username
read -p "Enter your Azure PostgreSQL admin username: " USERNAME
if [ -z "$USERNAME" ]; then
    echo "❌ Username is required"
    exit 1
fi

# Get password
read -s -p "Enter your Azure PostgreSQL admin password: " PASSWORD
echo ""
if [ -z "$PASSWORD" ]; then
    echo "❌ Password is required"
    exit 1
fi

# Get database name
read -p "Enter database name (default: ExchangeSystem): " DATABASE_NAME
DATABASE_NAME=${DATABASE_NAME:-ExchangeSystem}

echo ""
echo "🔧 Updating configuration files..."

# Update appsettings.Azure.json
sed -i.bak "s/YOUR_AZURE_SERVER/$SERVER_NAME/g" appsettings.Azure.json
sed -i.bak "s/YOUR_USERNAME/$USERNAME/g" appsettings.Azure.json
sed -i.bak "s/YOUR_PASSWORD/$PASSWORD/g" appsettings.Azure.json
sed -i.bak "s/ExchangeSystem/$DATABASE_NAME/g" appsettings.Azure.json

# Remove backup files
rm -f appsettings.Azure.json.bak

echo "✅ Configuration files updated"

# Test connection
echo ""
echo "🔍 Testing connection to Azure PostgreSQL..."

# Create connection string for testing
CONNECTION_STRING="Host=$SERVER_NAME.postgres.database.azure.com;Database=$DATABASE_NAME;Username=$USERNAME;Password=$PASSWORD;Port=5432;SslMode=Require;"

# Test with psql if available
if command -v psql &> /dev/null; then
    echo "Testing with psql..."
    if psql "$CONNECTION_STRING" -c "SELECT version();" &> /dev/null; then
        echo "✅ Connection successful!"
    else
        echo "❌ Connection failed. Please check your credentials and firewall settings."
        echo "💡 Make sure to add your IP address to the Azure PostgreSQL firewall rules."
    fi
else
    echo "⚠️  psql not found. Skipping connection test."
    echo "💡 Install PostgreSQL client to test connection: brew install postgresql"
fi

echo ""
echo "🎯 Next steps:"
echo "1. Run the application with Azure configuration:"
echo "   export ASPNETCORE_ENVIRONMENT=Azure"
echo "   dotnet run --urls \"https://localhost:5001;http://localhost:5002\""
echo ""
echo "2. Apply database migrations:"
echo "   dotnet ef database update"
echo ""
echo "3. Add test data:"
echo "   psql \"$CONNECTION_STRING\" -f add-test-data.sql"
echo ""
echo "4. Open browser: https://localhost:5001"
echo "   Login with: admin/admin123"
echo ""
echo "🔧 Configuration files updated:"
echo "   - appsettings.Azure.json"
echo ""
echo "📚 For more help, see: AZURE_SETUP_GUIDE.md"




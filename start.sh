#!/bin/bash

# Exchange System Startup Script

echo "🚀 Запуск системы обмена данными..."

# Check if .NET 8.0 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8.0 не установлен. Пожалуйста, установите .NET 8.0 SDK."
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET версия: $DOTNET_VERSION"

# Check if PostgreSQL is running
if ! pg_isready -q; then
    echo "⚠️  PostgreSQL не запущен. Пожалуйста, запустите PostgreSQL."
    echo "   На Ubuntu/Debian: sudo systemctl start postgresql"
    echo "   На macOS: brew services start postgresql"
    echo "   На Windows: net start postgresql-x64-14"
fi

# Restore packages
echo "📦 Восстановление пакетов..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "❌ Ошибка при восстановлении пакетов"
    exit 1
fi

# Build the project
echo "🔨 Сборка проекта..."
dotnet build

if [ $? -ne 0 ]; then
    echo "❌ Ошибка при сборке проекта"
    exit 1
fi

# Run the application
echo "🌐 Запуск приложения..."
echo "   URL: https://localhost:5001"
echo "   Логин: admin"
echo "   Пароль: admin123"
echo ""
echo "Нажмите Ctrl+C для остановки"

dotnet run




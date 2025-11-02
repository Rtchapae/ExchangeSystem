#!/bin/bash

# Exchange System Setup Script

echo "🚀 Настройка системы обмена данными..."

# Check if .NET 8.0 is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8.0 не установлен. Пожалуйста, установите .NET 8.0 SDK."
    echo "   Скачайте с: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "✅ .NET версия: $DOTNET_VERSION"

# Check if PostgreSQL is installed
if ! command -v psql &> /dev/null; then
    echo "❌ PostgreSQL не установлен. Пожалуйста, установите PostgreSQL."
    echo "   Ubuntu/Debian: sudo apt-get install postgresql postgresql-contrib"
    echo "   macOS: brew install postgresql"
    echo "   Windows: https://www.postgresql.org/download/windows/"
    exit 1
fi

# Check if PostgreSQL is running
if ! pg_isready -q; then
    echo "⚠️  PostgreSQL не запущен. Пожалуйста, запустите PostgreSQL."
    echo "   Ubuntu/Debian: sudo systemctl start postgresql"
    echo "   macOS: brew services start postgresql"
    echo "   Windows: net start postgresql-x64-14"
    exit 1
fi

echo "✅ PostgreSQL запущен"

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

# Setup database
echo "🗄️  Настройка базы данных..."
echo "   Введите пароль для пользователя postgres:"
psql -U postgres -f setup-database.sql

if [ $? -ne 0 ]; then
    echo "❌ Ошибка при настройке базы данных"
    echo "   Убедитесь, что PostgreSQL запущен и пароль правильный"
    exit 1
fi

echo "✅ База данных настроена"

# Run Entity Framework migrations
echo "🔄 Применение миграций Entity Framework..."
dotnet ef database update

if [ $? -ne 0 ]; then
    echo "⚠️  Миграции не применены (возможно, база данных уже настроена)"
fi

echo ""
echo "🎉 Настройка завершена!"
echo ""
echo "Для запуска приложения выполните:"
echo "   ./start.sh"
echo ""
echo "Или вручную:"
echo "   dotnet run"
echo ""
echo "После запуска откройте браузер:"
echo "   https://localhost:5001"
echo ""
echo "Тестовые данные для входа:"
echo "   Имя пользователя: admin"
echo "   Пароль: admin123"
echo ""




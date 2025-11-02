# Скрипт подключения к Azure PostgreSQL
# Замените значения на ваши реальные данные из Azure Portal

$serverName = "your-server.postgres.database.azure.com"
$databaseName = "ExchangeSystem"
$username = "your-username@your-server"
$password = "your-password"
$port = 5432

# Формируем строку подключения
$connectionString = "host=$serverName port=$port dbname=$databaseName user=$username password=$password sslmode=require"

Write-Host "Подключение к Azure PostgreSQL..." -ForegroundColor Green
Write-Host "Server: $serverName" -ForegroundColor Yellow
Write-Host "Database: $databaseName" -ForegroundColor Yellow
Write-Host ""
Write-Host "Используйте одну из команд ниже:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Через psql (если установлен):" -ForegroundColor White
Write-Host "psql `"$connectionString`"" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Через pgAdmin:" -ForegroundColor White
Write-Host "   - Host: $serverName" -ForegroundColor Gray
Write-Host "   - Port: $port" -ForegroundColor Gray
Write-Host "   - Database: $databaseName" -ForegroundColor Gray
Write-Host "   - Username: $username" -ForegroundColor Gray
Write-Host "   - SSL Mode: Require" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Или используйте Azure Portal -> Query editor" -ForegroundColor White


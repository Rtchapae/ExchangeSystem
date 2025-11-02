# 🔧 Настройка подключения к Azure PostgreSQL

## ❌ Проблема: "unknown host" или "127.0.0.1:5432"

**Ошибка возникает, потому что:**
- `127.0.0.1` или `localhost` работает только для локальной БД
- Для Azure PostgreSQL нужен полный DNS-адрес сервера
- Формат username должен быть: `username@servername`

## ✅ Решение: Правильная строка подключения

### Шаг 1: Получите данные из Azure Portal

1. Откройте [Azure Portal](https://portal.azure.com)
2. Найдите ваш **PostgreSQL сервер**
3. В разделе **Overview** найдите:
   - **Server name** (например: `exchangesystem-db`)
   - **Server admin login name** (например: `postgresadmin`)
4. В меню слева выберите **Connection strings**
5. Скопируйте данные или используйте формат ниже

### Шаг 2: Формат строки подключения для .NET/Npgsql

```
Host=YOUR_SERVER_NAME.postgres.database.azure.com;Database=ExchangeSystem;Username=YOUR_USERNAME@YOUR_SERVER_NAME;Password=YOUR_PASSWORD;Port=5432;SslMode=Require;
```

**Пример:**
Если ваш сервер называется `exchangesystem-db`, а логин `postgresadmin`:
```
Host=exchangesystem-db.postgres.database.azure.com;Database=ExchangeSystem;Username=postgresadmin@exchangesystem-db;Password=MyPassword123!;Port=5432;SslMode=Require;
```

### Шаг 3: Обновите appsettings.Azure.json

Замените `YOUR_SERVER_NAME`, `YOUR_USERNAME`, `YOUR_PASSWORD` на реальные значения:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=exchangesystem-db.postgres.database.azure.com;Database=ExchangeSystem;Username=postgresadmin@exchangesystem-db;Password=MyPassword123!;Port=5432;SslMode=Require;"
  }
}
```

### Шаг 4: Настройте Firewall в Azure

**⚠️ ВАЖНО!** Без этого подключение не будет работать!

1. В Azure Portal → ваш PostgreSQL сервер
2. В меню слева выберите **Connection security** или **Networking**
3. Добавьте правило firewall:
   - **Rule name**: `AllowMyIP` (любое имя)
   - **Start IP address**: ваш текущий IP (можно узнать на [whatismyip.com](https://whatismyip.com))
   - **End IP address**: тот же IP
   - Нажмите **Save**
4. Или включите **Allow access to Azure services** (если приложение в Azure)

### Шаг 5: Используйте правильный appsettings

**Для локальной разработки:**
```bash
dotnet run
```
Использует `appsettings.json` (localhost)

**Для Azure:**
```bash
dotnet run --environment Production
```
Или установите переменную окружения:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

Или в Visual Studio: Properties → launchSettings.json → измените `ASPNETCORE_ENVIRONMENT`

## 🔍 Проверка подключения

### Способ 1: Через Azure Portal Query Editor
1. Azure Portal → ваш PostgreSQL сервер
2. **Query editor** (в меню слева)
3. Войдите с учетными данными
4. Выполните: `SELECT version();`

### Способ 2: Через pgAdmin
1. Установите [pgAdmin](https://www.pgadmin.org/)
2. Создайте новое подключение:
   - **Name**: любое имя
   - **Host**: `your-server.postgres.database.azure.com`
   - **Port**: `5432`
   - **Database**: `ExchangeSystem`
   - **Username**: `your-username@your-server`
   - **Password**: ваш пароль
   - **SSL Mode**: `Require`
3. Сохраните и подключитесь

### Способ 3: Тест из приложения
Добавьте в `Program.cs` перед `app.Run()`:

```csharp
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ExchangeDbContext>();
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"Database connection: {(canConnect ? "✅ SUCCESS" : "❌ FAILED")}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database connection error: {ex.Message}");
}
```

## 📋 Чеклист для решения проблемы

- [ ] Получил правильное имя сервера из Azure Portal (формат: `server.postgres.database.azure.com`)
- [ ] Использую правильный формат username: `username@servername`
- [ ] Указал `SslMode=Require` в строке подключения
- [ ] Добавил свой IP в Firewall rules в Azure Portal
- [ ] Или включил "Allow access to Azure services"
- [ ] Проверил, что используется правильный файл appsettings (Azure, не localhost)
- [ ] Проверил пароль (он должен быть правильным)
- [ ] Попробовал подключиться через Azure Portal Query Editor

## 🚨 Частые ошибки

### Ошибка 1: "unknown host"
**Причина**: Используется `127.0.0.1` или неправильное имя сервера
**Решение**: Используйте полное имя: `server.postgres.database-count.azure.com`

### Ошибка 2: "password authentication failed"
**Причина**: Неправильный username или password
**Решение**: Проверьте формат username: `username@servername` (не просто `username`)

### Ошибка 3: "connection timeout" или "connection refused"
**Причина**: Firewall блокирует подключение
**Решение**: Добавьте ваш IP в Firewall rules в Azure Portal

### Ошибка 4: "SSL connection required"
**Причина**: Не указан `SslMode=Require`
**Решение**: Добавьте `SslMode=Require` в строку подключения

## 📞 Дополнительная помощь

- **Azure PostgreSQL Docs**: [Документация Azure PostgreSQL](https://docs.microsoft.com/en-us/azure/postgresql/)
- **Npgsql Connection String**: [Документация Npgsql](https://www.npgsql.org/doc/connection-string-parameters.html)


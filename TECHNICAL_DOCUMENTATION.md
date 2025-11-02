# Техническая документация - Система обмена данными

## Обзор системы

Система обмена данными - это веб-приложение на .NET 8.0, предназначенное для импорта, обработки и управления данными из CSV и DBF файлов. Система поддерживает русский язык и включает в себя аутентификацию, CRUD операции и автоматическое объединение данных.

## Архитектура

### Технологический стек
- **Backend**: .NET 8.0, ASP.NET Core Web API
- **Frontend**: HTML5, CSS3, JavaScript (ES6+), Bootstrap 5
- **База данных**: PostgreSQL
- **ORM**: Entity Framework Core
- **Аутентификация**: JWT Bearer Token
- **Файловые форматы**: CSV (Windows-1251), DBF (dBASE IV)

### Структура проекта
```
ExchangeSystem/
├── Controllers/          # API контроллеры
├── Models/              # Модели данных
├── Services/            # Бизнес-логика
├── Data/               # Контекст базы данных
├── Middleware/         # Промежуточное ПО
├── Views/              # Razor представления
├── wwwroot/            # Статические файлы
└── Program.cs          # Точка входа приложения
```

## Модели данных

### User (Пользователь)
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLoginAt { get; set; }
    public string Role { get; set; }
}
```

### Product (Продукт)
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Category { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public decimal? Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }
}
```

### Store (Магазин)
```csharp
public class Store
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Manager { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; }
}
```

### Transaction (Транзакция)
```csharp
public class Transaction
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? DocumentNumber { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal TotalAmount => Quantity * Price;
    public string? Supplier { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual Product Product { get; set; }
    public virtual Store Store { get; set; }
}
```

## API Endpoints

### Аутентификация
- `POST /api/auth/login` - Вход в систему
- `POST /api/auth/validate` - Валидация токена

### Импорт данных
- `POST /api/import/csv` - Импорт CSV файлов
- `POST /api/import/dbf` - Импорт DBF файлов
- `POST /api/import/validate-csv` - Валидация заголовков CSV
- `POST /api/import/validate-dbf` - Валидация заголовков DBF

### Управление данными
- `GET /api/data/joined` - Получение объединенных данных
- `POST /api/data/join` - Объединение данных
- `POST /api/data/regenerate-keys` - Генерация ключей
- `GET /api/data/products` - Получение списка продуктов
- `GET /api/data/stores` - Получение списка магазинов
- `GET /api/data/transactions` - Получение списка транзакций

## Импорт данных

### CSV файлы
Система поддерживает импорт CSV файлов с разделителем ";" и кодировкой Windows-1251. Автоматически определяется тип данных по заголовкам:

**Продукты:**
- название продукта
- категория
- код продукта
- описание
- единица
- цена

**Магазины:**
- название магазина
- адрес
- город
- телефон
- менеджер

**Транзакции:**
- дата транзакции
- название продукта
- название магазина
- номер документа
- количество
- цена
- поставщик
- срок годности
- примечания

### DBF файлы
Поддерживаются файлы в формате dBASE IV. Система автоматически определяет тип данных по содержимому и именам полей.

## Объединение данных

Система автоматически объединяет данные из разных источников:
1. Сопоставляет продукты по названию
2. Сопоставляет магазины по названию
3. Создает транзакции с связями
4. Генерирует уникальные ключи для записей, которые не удается объединить

### Алгоритм генерации ключей
```csharp
private string GenerateCustomKey(Transaction transaction)
{
    var keyComponents = new[]
    {
        transaction.ProductId.ToString(),
        transaction.StoreId.ToString(),
        transaction.TransactionDate.ToString("yyyyMMdd"),
        transaction.DocumentNumber ?? "",
        transaction.Quantity.ToString("F3"),
        transaction.Price.ToString("F2")
    };

    var keyString = string.Join("_", keyComponents);
    return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyString))[..12];
}
```

## Безопасность

### Аутентификация
- JWT Bearer Token аутентификация
- Хеширование паролей с использованием BCrypt
- Токены действительны 8 часов

### Авторизация
- Защищенные API endpoints требуют валидный JWT токен
- Роли пользователей (Admin, User)

### Валидация данных
- Валидация входных данных на уровне моделей
- Санитизация файлов при импорте
- Обработка ошибок и исключений

## База данных

### PostgreSQL
- Версия: 12+
- Кодировка: UTF-8
- Поддержка русских символов

### Миграции
```bash
# Создание миграции
dotnet ef migrations add InitialCreate

# Применение миграций
dotnet ef database update
```

### Индексы
- Уникальный индекс на код продукта
- Уникальный индекс на имя пользователя
- Уникальный индекс на email пользователя

## Развертывание

### Требования
- .NET 8.0 Runtime
- PostgreSQL 12+
- Веб-сервер (IIS, Nginx, Apache)

### Конфигурация
1. Обновите строку подключения в `appsettings.json`
2. Настройте JWT ключи
3. Запустите миграции базы данных

### Переменные окружения
```bash
ConnectionStrings__DefaultConnection="Host=localhost;Database=ExchangeSystem;Username=postgres;Password=password"
Jwt__Key="YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
Jwt__Issuer="ExchangeSystem"
Jwt__Audience="ExchangeSystemUsers"
```

## Мониторинг и логирование

### Логирование
- Встроенное логирование .NET
- Уровни: Information, Warning, Error
- Логирование в консоль и файлы

### Обработка ошибок
- Глобальный обработчик исключений
- Структурированные ответы об ошибках
- Логирование всех исключений

## Тестирование

### Тестовые данные
- Администратор: `admin` / `admin123`
- Автоматическое создание тестовых данных при первом запуске

### API тестирование
- Swagger UI доступен в режиме разработки
- Документация API с примерами

## Производительность

### Оптимизации
- Асинхронные операции
- Пагинация для больших наборов данных
- Индексы базы данных
- Кэширование статических ресурсов

### Масштабируемость
- Stateless архитектура
- Горизонтальное масштабирование
- Поддержка балансировки нагрузки

## Поддержка и обслуживание

### Резервное копирование
- Регулярное резервное копирование базы данных
- Экспорт данных в CSV/JSON форматы

### Обновления
- Миграции базы данных
- Обратная совместимость API
- Версионирование схемы данных

## Контакты

Для технической поддержки и вопросов по системе обращайтесь к команде разработки.

---

*Документация обновлена: 2025-01-03*




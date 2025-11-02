# 📚 Полное руководство по CRUD операциям в ExchangeSystem

## 🎯 Структура приложения

```
ExchangeSystem/
├── Controllers/          # API контроллеры (Backend)
│   ├── DataController.cs      # ✅ Главный контроллер с CRUD
│   ├── ProductsController.cs  # CRUD для продуктов
│   ├── StoresController.cs    # CRUD для магазинов
│   └── TransactionsController.cs # CRUD для транзакций
├── Services/            # Бизнес-логика
│   ├── ProductService.cs
│   ├── StoreService.cs
│   └── TransactionService.cs
├── Models/              # Модели данных
│   ├── Product.cs
│   ├── Store.cs
│   ├── Transaction.cs
│   └── User.cs
├── Views/Home/          # Frontend страницы
│   ├── Products.cshtml       # ✅ UI для CRUD продуктов
│   ├── Stores.cshtml         # ✅ UI для CRUD магазинов
│   └── Transactions.cshtml   # ✅ UI для CRUD транзакций
└── wwwroot/js/          # JavaScript
    └── api.js                # API клиент
```

---

## 🔧 1. Backend CRUD API

### **Файл: `/Controllers/DataController.cs`**

Это главный API контроллер со всеми CRUD операциями.

#### **📦 PRODUCTS (Продукты)**

**Базовый URL**: `/api/data/products`

| Метод | Endpoint | Описание | Тело запроса |
|-------|----------|----------|--------------|
| `GET` | `/api/data/products` | Получить все продукты | - |
| `GET` | `/api/data/products?search=текст` | Поиск продуктов | - |
| `GET` | `/api/data/products/{id}` | Получить продукт по ID | - |
| `POST` | `/api/data/products` | Создать новый продукт | JSON с данными |
| `PUT` | `/api/data/products/{id}` | Обновить продукт | JSON с данными |
| `DELETE` | `/api/data/products/{id}` | Удалить продукт | - |

**Пример тела запроса (JSON)**:
```json
{
  "name": "Молоко",
  "code": "MILK001",
  "category": "Молочные продукты",
  "price": 75.50,
  "unit": "л",
  "description": "Молоко пастеризованное 3.2%"
}
```

#### **🏪 STORES (Магазины)**

**Базовый URL**: `/api/data/stores`

| Метод | Endpoint | Описание | Тело запроса |
|-------|----------|----------|--------------|
| `GET` | `/api/data/stores` | Получить все магазины | - |
| `GET` | `/api/data/stores?search=текст` | Поиск магазинов | - |
| `GET` | `/api/data/stores/{id}` | Получить магазин по ID | - |
| `POST` | `/api/data/stores` | Создать новый магазин | JSON с данными |
| `PUT` | `/api/data/stores/{id}` | Обновить магазин | JSON с данными |
| `DELETE` | `/api/data/stores/{id}` | Удалить магазин | - |

**Пример тела запроса (JSON)**:
```json
{
  "name": "Магазин №1",
  "address": "ул. Ленина, 10",
  "city": "Минск",
  "phone": "+375291234567",
  "manager": "Иванов И.И."
}
```

#### **💰 TRANSACTIONS (Транзакции)**

**Базовый URL**: `/api/data/transactions`

| Метод | Endpoint | Описание | Тело запроса |
|-------|----------|----------|--------------|
| `GET` | `/api/data/transactions` | Получить все транзакции | - |
| `GET` | `/api/data/transactions/{id}` | Получить транзакцию по ID | - |
| `POST` | `/api/data/transactions` | Создать новую транзакцию | JSON с данными |
| `PUT` | `/api/data/transactions/{id}` | Обновить транзакцию | JSON с данными |
| `DELETE` | `/api/data/transactions/{id}` | Удалить транзакцию | - |

**Пример тела запроса (JSON)**:
```json
{
  "productId": 1,
  "storeId": 2,
  "quantity": 100,
  "price": 75.50,
  "transactionDate": "2025-01-15",
  "documentNumber": "DOC-2025-001",
  "supplier": "ООО Поставщик",
  "expiryDate": "2025-02-15",
  "notes": "Партия №123"
}
```

---

## 🖥️ 2. Frontend UI (Razor Views)

### **Products Page** - `/Views/Home/Products.cshtml`

**URL в браузере**: `https://localhost:5001/Home/Products`

**Функции**:
- ✅ Просмотр всех продуктов (таблица)
- ✅ Поиск продуктов
- ✅ Создание нового продукта (кнопка "Добавить продукт")
- ✅ Редактирование продукта (кнопка "✏️")
- ✅ Удаление продукта (кнопка "🗑️")
- ✅ Категория - выпадающий список (dropdown)

**Скриншот функционала**:
```
┌─────────────────────────────────────────────────┐
│ 📦 Управление продуктами                        │
│ [Поиск...] [🔍] [+ Добавить продукт]           │
├─────────────────────────────────────────────────┤
│ ID │ Название │ Код │ Категория │ Цена │ Ед. │ │
│ 1  │ Молоко   │ M01 │ Молочные  │ 75₽  │ л   │ ✏️🗑️ │
│ 2  │ Хлеб     │ B01 │ Хлеб      │ 40₽  │ шт  │ ✏️🗑️ │
└─────────────────────────────────────────────────┘
```

### **Stores Page** - `/Views/Home/Stores.cshtml`

**URL в браузере**: `https://localhost:5001/Home/Stores`

**Функции**:
- ✅ Просмотр всех магазинов (таблица)
- ✅ Поиск магазинов
- ✅ Создание нового магазина
- ✅ Редактирование магазина
- ✅ Удаление магазина

### **Transactions Page** - `/Views/Home/Transactions.cshtml`

**URL в браузере**: `https://localhost:5001/Home/Transactions`

**Функции**:
- ✅ Просмотр всех транзакций
- ✅ Фильтрация по дате
- ✅ Создание новой транзакции
- ✅ Редактирование транзакции
- ✅ Удаление транзакции
- ✅ Отображение связанных данных (продукт + магазин)

---

## 💻 3. Как работают CRUD операции

### **Пример: Создание продукта**

**Шаг 1: Пользователь заполняет форму**
```
Products.cshtml (UI) → Нажимает "Сохранить"
```

**Шаг 2: JavaScript отправляет запрос**
```javascript
// В файле: /wwwroot/js/api.js
const response = await api.post('/data/products', {
    name: "Молоко",
    code: "MILK001",
    category: "Молочные продукты",
    price: 75.50
});
```

**Шаг 3: Backend обрабатывает запрос**
```csharp
// В файле: /Controllers/DataController.cs
[HttpPost("products")]
public async Task<IActionResult> CreateProduct([FromBody] Product product)
{
    var createdProduct = await _productService.CreateProductAsync(product);
    return Ok(new { success = true, data = createdProduct });
}
```

**Шаг 4: Service сохраняет в БД**
```csharp
// В файле: /Services/ProductService.cs
public async Task<Product> CreateProductAsync(Product product)
{
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
    return product;
}
```

---

## 🗄️ 4. Как открыть и посмотреть базу данных

### **Option 1: Command Line (psql)**

```bash
# Подключиться к базе данных
psql -d ExchangeSystem -U romanukhnalev

# Команды в psql:
\dt                          # Показать все таблицы
\d "Products"                # Структура таблицы Products
SELECT * FROM "Products";    # Все продукты
SELECT * FROM "Stores";      # Все магазины
SELECT * FROM "Transactions"; # Все транзакции
\q                           # Выход
```

### **Option 2: Azure Data Studio**

**Настройки подключения**:
```
Server name: localhost
Authentication type: Password
User name: romanukhnalev
Password: [ваш пароль]
Database name: ExchangeSystem
Port: 5432
SSL Mode: Disable
```

### **Option 3: pgAdmin**

1. Установить: `brew install --cask pgadmin4`
2. Открыть pgAdmin
3. Добавить сервер:
   - Host: localhost
   - Port: 5432
   - Database: ExchangeSystem
   - Username: romanukhnalev

---

## 📍 5. Прямые ссылки на CRUD в вашем приложении

После запуска приложения (`dotnet run`):

### **Frontend (UI страницы)**
- 🏠 **Dashboard**: https://localhost:5001/Home/Dashboard
- 📦 **Products CRUD**: https://localhost:5001/Home/Products
- 🏪 **Stores CRUD**: https://localhost:5001/Home/Stores
- 💰 **Transactions CRUD**: https://localhost:5001/Home/Transactions
- 📤 **Import**: https://localhost:5001/Home/Import

### **Backend API (REST endpoints)**
- 📦 **Products API**: https://localhost:5001/api/data/products
- 🏪 **Stores API**: https://localhost:5001/api/data/stores
- 💰 **Transactions API**: https://localhost:5001/api/data/transactions

---

## 🔐 6. Аутентификация

**Логин**: `admin`  
**Пароль**: `admin123`

После входа в систему вы получите JWT токен, который автоматически используется для всех API запросов.

---

## 📊 7. Структура таблиц в БД

### **Products (Продукты)**
```sql
CREATE TABLE "Products" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Code" TEXT NOT NULL,
    "Category" TEXT,
    "Price" DECIMAL(18,2),
    "Unit" TEXT,
    "Description" TEXT,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP
);
```

### **Stores (Магазины)**
```sql
CREATE TABLE "Stores" (
    "Id" INTEGER PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Address" TEXT,
    "City" TEXT,
    "Phone" TEXT,
    "Manager" TEXT,
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP
);
```

### **Transactions (Транзакции)**
```sql
CREATE TABLE "Transactions" (
    "Id" INTEGER PRIMARY KEY,
    "ProductId" INTEGER NOT NULL,
    "StoreId" INTEGER NOT NULL,
    "Quantity" INTEGER,
    "Price" DECIMAL(18,2),
    "TransactionDate" DATE,
    "DocumentNumber" TEXT,
    "Supplier" TEXT,
    "ExpiryDate" DATE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP,
    FOREIGN KEY ("ProductId") REFERENCES "Products"("Id"),
    FOREIGN KEY ("StoreId") REFERENCES "Stores"("Id")
);
```

---

## 🚀 8. Быстрый старт

### **Запустить приложение**:
```bash
cd /Users/romanukhnalev/Documents/ExchangeSystem
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:5001;http://localhost:5002"
```

### **Открыть в браузере**:
```
https://localhost:5001
```

### **Войти**:
- Логин: `admin`
- Пароль: `admin123`

### **Использовать CRUD**:
1. Перейти на страницу "Продукты" или "Магазины"
2. Нажать "+ Добавить продукт/магазин"
3. Заполнить форму
4. Нажать "Сохранить"
5. Продукт появится в таблице
6. Можно редактировать (✏️) или удалить (🗑️)

---

## 📝 9. Файлы с CRUD логикой

### **Backend**:
- `/Controllers/DataController.cs` - Все API endpoints
- `/Services/ProductService.cs` - Логика для продуктов
- `/Services/StoreService.cs` - Логика для магазинов
- `/Services/TransactionService.cs` - Логика для транзакций

### **Frontend**:
- `/Views/Home/Products.cshtml` - UI продуктов
- `/Views/Home/Stores.cshtml` - UI магазинов
- `/Views/Home/Transactions.cshtml` - UI транзакций
- `/wwwroot/js/api.js` - HTTP клиент для API

### **Database**:
- `/Data/ExchangeDbContext.cs` - Entity Framework контекст
- `/Models/Product.cs` - Модель продукта
- `/Models/Store.cs` - Модель магазина
- `/Models/Transaction.cs` - Модель транзакции

---

## ✅ Готово!

Теперь у вас есть полное понимание:
- ✅ Где находятся CRUD операции
- ✅ Как они работают
- ✅ Как открыть базу данных
- ✅ Как пользоваться UI
- ✅ Какие API endpoints существуют

**Приложение полностью функционально и готово к использованию!** 🎉



# 🗄️ Как подключиться к PostgreSQL базе данных

## 📊 Информация о вашей базе данных

**Имя базы**: `ExchangeSystem`  
**Пользователь**: `romanukhnalev`  
**Хост**: `localhost`  
**Порт**: `5432`  

---

## 🔧 Azure Data Studio - Правильные настройки

### **Шаг 1: Открыть Azure Data Studio**

### **Шаг 2: Нажать "New Connection"**

### **Шаг 3: Заполнить поля**:

```
Connection type: PostgreSQL ✅

Server name: localhost
                ИЛИ
             127.0.0.1

Authentication type: Password (NOT Azure Active Directory!)

User name: romanukhnalev

Password: [оставить пустым если нет пароля]

Database name: ExchangeSystem

Port: 5432

SSL Mode: Disable  ⚠️ ВАЖНО!

Trust server certificate: ✅ (отметить галочкой)
```

### **Шаг 4: Нажать "Connect"**

---

## ❌ Ваша ошибка

Вы использовали неправильный формат:
```
Host=localhost;Port=5432;Database=ExchangeSystem;Username=romanukhnalev;Ssl Mode=Disable
```

Это формат для **connection string**, а не для полей Azure Data Studio!

---

## ✅ Альтернатива 1: psql (Command Line)

```bash
# Простое подключение
psql -d ExchangeSystem -U romanukhnalev

# После подключения:
\dt                           # Показать все таблицы
SELECT * FROM "Products";     # Посмотреть продукты
SELECT * FROM "Stores";       # Посмотреть магазины
SELECT * FROM "Transactions"; # Посмотреть транзакции
\q                            # Выход
```

---

## ✅ Альтернатива 2: pgAdmin 4

### **Установка**:
```bash
brew install --cask pgadmin4
```

### **Настройка**:
1. Открыть pgAdmin
2. Right-click "Servers" → "Register" → "Server"
3. Заполнить:
   - **General** tab:
     - Name: `ExchangeSystem Local`
   - **Connection** tab:
     - Host: `localhost`
     - Port: `5432`
     - Database: `ExchangeSystem`
     - Username: `romanukhnalev`
     - Password: [оставить пустым]
4. Нажать "Save"

---

## ✅ Альтернатива 3: DBeaver (Universal Database Tool)

### **Установка**:
```bash
brew install --cask dbeaver-community
```

### **Настройка**:
1. New Connection → PostgreSQL
2. Заполнить:
   - Host: `localhost`
   - Port: `5432`
   - Database: `ExchangeSystem`
   - Username: `romanukhnalev`
   - Password: [оставить пустым]
3. Test Connection → OK → Finish

---

## 🔍 Проверка подключения (Terminal)

```bash
# Проверить что PostgreSQL запущен
pg_isready

# Посмотреть список баз данных
psql -l

# Должны увидеть:
#   ExchangeSystem | romanukhnalev | ...
```

---

## 📊 Полезные SQL запросы

### **Посмотреть все продукты**:
```sql
SELECT * FROM "Products" WHERE "IsActive" = true ORDER BY "Name";
```

### **Посмотреть все магазины**:
```sql
SELECT * FROM "Stores" WHERE "IsActive" = true ORDER BY "Name";
```

### **Посмотреть транзакции с деталями**:
```sql
SELECT 
    t."Id",
    p."Name" as "ProductName",
    s."Name" as "StoreName",
    t."Quantity",
    t."Price",
    t."TransactionDate"
FROM "Transactions" t
JOIN "Products" p ON t."ProductId" = p."Id"
JOIN "Stores" s ON t."StoreId" = s."Id"
ORDER BY t."TransactionDate" DESC;
```

### **Статистика**:
```sql
-- Количество продуктов
SELECT COUNT(*) as "Total Products" FROM "Products" WHERE "IsActive" = true;

-- Количество магазинов
SELECT COUNT(*) as "Total Stores" FROM "Stores" WHERE "IsActive" = true;

-- Количество транзакций
SELECT COUNT(*) as "Total Transactions" FROM "Transactions";
```

---

## ⚠️ Если не можете подключиться

### **Проверка 1: PostgreSQL работает?**
```bash
brew services list | grep postgresql
# Должно показать: postgresql@14 started
```

### **Проверка 2: База данных существует?**
```bash
psql -l | grep ExchangeSystem
# Должно показать: ExchangeSystem | romanukhnalev
```

### **Проверка 3: Перезапустить PostgreSQL**
```bash
brew services restart postgresql@14
```

---

## 🎯 Рекомендация

Для быстрого просмотра данных лучше всего использовать:
1. **psql** - быстро и просто
2. **pgAdmin** - полнофункциональный GUI
3. **DBeaver** - универсальный инструмент

Azure Data Studio хороша для Azure PostgreSQL, но для локальной БД лучше использовать другие инструменты.



# Пример использования интеграционного модуля

## 1. Запуск приложения

```bash
cd /Users/romanukhnalev/Documents/ExchangeSystem
dotnet run
```

Приложение будет доступно по адресу: `https://localhost:5001`

## 2. Авторизация

1. Откройте браузер и перейдите по адресу `https://localhost:5001`
2. Введите логин: `admin`
3. Введите пароль: `admin123`
4. Нажмите "Войти"

## 3. Импорт данных потребления

### Шаг 1: Подготовка CSV файла
Создайте CSV файл с данными в следующем формате:

```csv
"id продукта";"Шифр продукта";"Название продукта";ед.изм.;"Дата расхода";"Ясли 10,5 к-во";"Ясли 10,5 стоимость";"САД 10,5 к-во";"САД 10,5 стоимость";"Сотрудники к-во";"Сотрудники стоимость";"Всего к-во";"Всего стоимость"
213;610001;"Говядина (т/часть)";кг;2020-05-06;0.713;8.16;5.287;60.55;0;0;6;68.71
212;610002;"Свинина (т/ч)";кг;2020-05-07;0.394;3.99;1.606;16.28;0;0;2;20.27
```

### Шаг 2: Импорт через веб-интерфейс
1. Перейдите в раздел "Импорт данных"
2. В секции "Импорт данных потребления" нажмите "Выберите CSV файл"
3. Выберите подготовленный CSV файл
4. Нажмите "Импортировать"
5. Дождитесь завершения импорта и просмотрите результаты

### Шаг 3: Проверка результатов
1. В таблице "История импорта" вы увидите запись о вашем импорте
2. Если были ошибки, нажмите "Показать ошибки" для их просмотра

## 4. Сопоставление продуктов

### Шаг 1: Переход к сопоставлению
1. Перейдите в раздел "Сопоставление продуктов"
2. Вы увидите список всех импортированных продуктов

### Шаг 2: Автоматическое сопоставление
1. Нажмите кнопку "Автоматическое сопоставление"
2. Система попытается автоматически сопоставить продукты по коду и названию
3. Просмотрите результаты сопоставления

### Шаг 3: Ручное сопоставление
1. Найдите несопоставленные продукты (статус "Не сопоставлено")
2. Нажмите "Редактировать" для ручного сопоставления
3. В поле "Поиск продукта в СВС" введите название или код продукта
4. Выберите соответствующий продукт из СВС
5. Отметьте "Утверждено" если сопоставление корректно
6. Нажмите "Сохранить"

## 5. Использование API для СВС

### Получение данных потребления
```bash
curl -X GET "https://localhost:5001/api/svs/consumption?organizationId=123&date=2020-05-06"
```

### Получение данных прихода
```bash
curl -X GET "https://localhost:5001/api/svs/receipts?organizationId=123&date=2020-05-06"
```

### Получение справочника продуктов
```bash
curl -X GET "https://localhost:5001/api/svs/products"
```

### Проверка доступности данных
```bash
curl -X GET "https://localhost:5001/api/svs/check-data-availability?date=2020-05-06"
```

## 6. Программное использование API

### Пример на C#
```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class SvsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public SvsApiClient(string baseUrl)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    public async Task<string> GetConsumptionDataAsync(string organizationId, DateTime date)
    {
        var url = $"{_baseUrl}/api/svs/consumption?organizationId={organizationId}&date={date:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetReceiptDataAsync(string organizationId, DateTime date)
    {
        var url = $"{_baseUrl}/api/svs/receipts?organizationId={organizationId}&date={date:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }
}
```

### Пример на JavaScript
```javascript
class SvsApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }

    async getConsumptionData(organizationId, date) {
        const url = `${this.baseUrl}/api/svs/consumption?organizationId=${organizationId}&date=${date}`;
        const response = await fetch(url);
        return await response.json();
    }

    async getReceiptData(organizationId, date) {
        const url = `${this.baseUrl}/api/svs/receipts?organizationId=${organizationId}&date=${date}`;
        const response = await fetch(url);
        return await response.json();
    }
}

// Использование
const client = new SvsApiClient('https://localhost:5001');
const consumptionData = await client.getConsumptionData('123', '2020-05-06');
console.log(consumptionData);
```

## 7. Мониторинг и отладка

### Просмотр логов импорта
1. Перейдите в раздел "Импорт данных"
2. В таблице "История импорта" найдите нужный импорт
3. Нажмите "Показать ошибки" для просмотра деталей

### Проверка статуса системы
```bash
curl -X GET "https://localhost:5001/api/svs/check-data-availability?date=2020-05-06"
```

Ответ:
```json
{
  "success": true,
  "date": "2020-05-06",
  "hasData": true,
  "hasConsumptionData": true,
  "hasReceiptData": false,
  "consumptionCount": 15,
  "receiptCount": 0
}
```

## 8. Типичные сценарии использования

### Сценарий 1: Ежедневный импорт данных
1. Ежедневно загружайте CSV файлы с данными потребления
2. Проверяйте результаты импорта
3. Исправляйте ошибки при необходимости
4. СВС получает данные через API

### Сценарий 2: Настройка сопоставлений
1. При первом использовании настройте сопоставления продуктов
2. Используйте автоматическое сопоставление для быстрой настройки
3. Ручная настройка для точного сопоставления
4. Утвердите сопоставления

### Сценарий 3: Интеграция с СВС
1. СВС запрашивает данные через API
2. ИМ возвращает данные в формате JSON
3. СВС обрабатывает данные и формирует проводки
4. Логирование всех операций

## 9. Устранение неполадок

### Ошибки импорта
- Проверьте формат CSV файла
- Убедитесь в корректности разделителей (точка с запятой)
- Проверьте кодировку файла (UTF-8)

### Ошибки сопоставления
- Проверьте наличие продуктов в справочнике СВС
- Используйте ручное сопоставление для сложных случаев
- Проверьте логи для диагностики

### Ошибки API
- Проверьте правильность параметров запроса
- Убедитесь в наличии данных на указанную дату
- Проверьте логи сервера для диагностики


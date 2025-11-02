using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public class CsvParserService : ICsvParserService
    {
        private readonly ILogger<CsvParserService> _logger;

        public CsvParserService(ILogger<CsvParserService> logger)
        {
            _logger = logger;
        }

        public Task<CsvParseResult> ParseConsumptionDataAsync(Stream csvStream, string fileName)
        {
            var result = new CsvParseResult();
            var parsedData = new List<ConsumptionDataRow>();

            try
            {
                using var reader = new StreamReader(csvStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                });

                csv.Context.RegisterClassMap<ConsumptionDataRowMap>();

                var records = csv.GetRecords<ConsumptionDataRow>();
                var rowNumber = 1;

                foreach (var record in records)
                {
                    rowNumber++;
                    result.TotalRows++;

                    try
                    {
                        // Валидация данных
                        if (string.IsNullOrEmpty(record.ProductName))
                        {
                            result.Errors.Add($"Строка {rowNumber}: Отсутствует название продукта");
                            result.ErrorRows++;
                            continue;
                        }

                        if (record.ConsumptionDate == default)
                        {
                            result.Errors.Add($"Строка {rowNumber}: Неверная дата расхода");
                            result.ErrorRows++;
                            continue;
                        }

                        parsedData.Add(record);
                        result.ProcessedRows++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Строка {rowNumber}: {ex.Message}");
                        result.ErrorRows++;
                    }
                }

                result.ParsedData = parsedData.Cast<object>().ToList();
                result.IsSuccess = result.ErrorRows == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при парсинге CSV файла {FileName}", fileName);
                result.Errors.Add($"Ошибка парсинга файла: {ex.Message}");
                result.IsSuccess = false;
            }

            return Task.FromResult(result);
        }

        public Task<CsvParseResult> ParseProductDataAsync(Stream csvStream, string fileName)
        {
            var result = new CsvParseResult();
            var parsedData = new List<object>();

            try
            {
                using var reader = new StreamReader(csvStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                });

                // Регистрация маппинга
                csv.Context.RegisterClassMap<ProductDataRowMap>();
                
                // Парсинг справочника продуктов
                var records = csv.GetRecords<ProductDataRow>();
                var rowNumber = 1;

                foreach (var record in records)
                {
                    rowNumber++;
                    result.TotalRows++;

                    try
                    {
                        // Валидация
                        if (string.IsNullOrWhiteSpace(record.ProductName))
                        {
                            throw new Exception("Отсутствует название продукта");
                        }
                        
                        parsedData.Add(record);
                        result.ProcessedRows++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Строка {rowNumber}: {ex.Message}");
                        result.ErrorRows++;
                    }
                }

                result.ParsedData = parsedData;
                result.IsSuccess = result.ErrorRows == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при парсинге CSV файла {FileName}", fileName);
                result.Errors.Add($"Ошибка парсинга файла: {ex.Message}");
                result.IsSuccess = false;
            }

            return Task.FromResult(result);
        }

        public Task<CsvParseResult> ParseReceiptDataAsync(Stream csvStream, string fileName)
        {
            var result = new CsvParseResult();
            var parsedData = new List<object>();

            try
            {
                using var reader = new StreamReader(csvStream, Encoding.UTF8);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    BadDataFound = null
                });

                // Парсинг данных прихода
                var records = csv.GetRecords<dynamic>();
                var rowNumber = 1;

                foreach (var record in records)
                {
                    rowNumber++;
                    result.TotalRows++;

                    try
                    {
                        // Валидация и обработка данных прихода
                        parsedData.Add(record);
                        result.ProcessedRows++;
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Строка {rowNumber}: {ex.Message}");
                        result.ErrorRows++;
                    }
                }

                result.ParsedData = parsedData;
                result.IsSuccess = result.ErrorRows == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при парсинге CSV файла {FileName}", fileName);
                result.Errors.Add($"Ошибка парсинга файла: {ex.Message}");
                result.IsSuccess = false;
            }

            return Task.FromResult(result);
        }
    }

    public class ConsumptionDataRowMap : ClassMap<ConsumptionDataRow>
    {
        public ConsumptionDataRowMap()
        {
            Map(m => m.ProductId).Name("id продукта");
            Map(m => m.ProductCode).Name("Шифр продукта");
            Map(m => m.ProductName).Name("Название продукта");
            Map(m => m.Unit).Name("ед.изм.");
            Map(m => m.ConsumptionDate).Name("Дата расхода").TypeConverter<DateTimeConverter>();
            Map(m => m.NurseryQuantity).Name("Ясли 10,5 к-во").TypeConverter<DecimalConverter>();
            Map(m => m.NurseryCost).Name("Ясли 10,5 стоимость").TypeConverter<DecimalConverter>();
            Map(m => m.KindergartenQuantity).Name("САД 10,5 к-во").TypeConverter<DecimalConverter>();
            Map(m => m.KindergartenCost).Name("САД 10,5 стоимость").TypeConverter<DecimalConverter>();
            Map(m => m.StaffQuantity).Name("Сотрудники к-во").TypeConverter<DecimalConverter>();
            Map(m => m.StaffCost).Name("Сотрудники стоимость").TypeConverter<DecimalConverter>();
            Map(m => m.TotalQuantity).Name("Всего к-во").TypeConverter<DecimalConverter>();
            Map(m => m.TotalCost).Name("Всего стоимость").TypeConverter<DecimalConverter>();
        }
    }

    public class ProductDataRowMap : ClassMap<ProductDataRow>
    {
        public ProductDataRowMap()
        {
            // Используем индексы столбцов, т.к. заголовки не совпадают с данными
            Map(m => m.ProductId).Index(0);      // "id продукта"
            Map(m => m.ProductName).Index(1);    // "Название продукта"
            Map(m => m.ProductCode).Index(2);    // "Шифр"
            Map(m => m.Category).Index(6);       // "Птица", "Мясо" и т.д. (7-й столбец)
            Map(m => m.Unit).Index(7);           // "кг", "шт" и т.д. (8-й столбец)
        }
    }

    public class DateTimeConverter : CsvHelper.TypeConversion.DateTimeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return default(DateTime);

            if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            if (DateTime.TryParse(text, out result))
                return result;

            return default(DateTime);
        }
    }

    public class DecimalConverter : CsvHelper.TypeConversion.DecimalConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrEmpty(text))
                return 0m;

            // Заменяем запятую на точку для корректного парсинга
            text = text.Replace(',', '.');
            
            if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0m;
        }
    }
}

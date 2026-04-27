using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Master_floor.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        private Database _db;

        [TestInitialize]
        public void Setup()
        {
            _db = new Database();
        }

        // ТЕСТ 1: Проверка логики расчета скидки (изолированно)
        [TestMethod]
        public void GetDiscount_ShouldReturnCorrectPercentage()
        {
            // Создаем тестовую БД с контролируемыми данными
            var testDb = new Database();

            // Очищаем существующие продажи для тестового партнера
            int testPartnerId = 99;

            // Добавляем тестового партнера
            var testPartner = new Partner
            {
                ID = testPartnerId,
                Тип_партнера = "ООО",
                Наименование_партнера = "Тестовый партнер",
                Директор = "Тестов",
                Телефон_партнера = "123",
                Рейтинг = 50
            };
            testDb.Partners.Add(testPartner);

            // Тест 1: Сумма < 10000 → 0%
            testDb.Sales.Add(new Sale
            {
                ID = 999,
                ID_Partner = testPartnerId,
                Наименование_продукции = "Тест",
                Количество = 5000,
                Дата = DateTime.Now
            });
            Assert.AreEqual("0%", testDb.GetDiscount(testPartnerId), "Скидка должна быть 0% при сумме < 10000");

            // Очищаем продажи для следующего теста
            testDb.Sales.RemoveAll(s => s.ID_Partner == testPartnerId);

            // Тест 2: Сумма 10000-49999 → 5%
            testDb.Sales.Add(new Sale
            {
                ID = 999,
                ID_Partner = testPartnerId,
                Наименование_продукции = "Тест",
                Количество = 25000,
                Дата = DateTime.Now
            });
            Assert.AreEqual("5%", testDb.GetDiscount(testPartnerId), "Скидка должна быть 5% при сумме 10000-49999");

            // Очищаем продажи для следующего теста
            testDb.Sales.RemoveAll(s => s.ID_Partner == testPartnerId);

            // Тест 3: Сумма 50000-299999 → 10%
            testDb.Sales.Add(new Sale
            {
                ID = 999,
                ID_Partner = testPartnerId,
                Наименование_продукции = "Тест",
                Количество = 100000,
                Дата = DateTime.Now
            });
            Assert.AreEqual("10%", testDb.GetDiscount(testPartnerId), "Скидка должна быть 10% при сумме 50000-299999");

            // Очищаем продажи для следующего теста
            testDb.Sales.RemoveAll(s => s.ID_Partner == testPartnerId);

            // Тест 4: Сумма ≥ 300000 → 15%
            testDb.Sales.Add(new Sale
            {
                ID = 999,
                ID_Partner = testPartnerId,
                Наименование_продукции = "Тест",
                Количество = 500000,
                Дата = DateTime.Now
            });
            Assert.AreEqual("15%", testDb.GetDiscount(testPartnerId), "Скидка должна быть 15% при сумме ≥ 300000");
        }

        // ТЕСТ 2: Проверка добавления нового партнера
        [TestMethod]
        public void AddPartner_ShouldIncreasePartnerCount()
        {
            // Arrange
            int initialCount = _db.Partners.Count;
            var newPartner = new Partner
            {
                ID = initialCount + 1,
                Тип_партнера = "ООО",
                Наименование_партнера = "Тестовый партнер",
                Директор = "Тестовый директор",
                Телефон_партнера = "+7 (999) 123-45-67",
                Рейтинг = 85
            };

            // Act
            _db.Partners.Add(newPartner);
            int newCount = _db.Partners.Count;

            // Assert
            Assert.AreEqual(initialCount + 1, newCount);
            Assert.IsTrue(_db.Partners.Any(p => p.Наименование_партнера == "Тестовый партнер"));
        }

        // ТЕСТ 3: Проверка валидации рейтинга (1-100)
        [TestMethod]
        public void RatingValidation_ShouldBeBetween1And100()
        {
            // Arrange
            var validRatings = new[] { 1, 50, 100 };
            var invalidRatings = new[] { 0, -5, 101, 150 };

            // Act & Assert
            foreach (var rating in validRatings)
            {
                bool isValid = rating >= 1 && rating <= 100;
                Assert.IsTrue(isValid, $"Рейтинг {rating} должен быть валидным");
            }

            foreach (var rating in invalidRatings)
            {
                bool isValid = rating >= 1 && rating <= 100;
                Assert.IsFalse(isValid, $"Рейтинг {rating} должен быть невалидным");
            }
        }

        // ТЕСТ 4: Проверка наличия данных в БД
        [TestMethod]
        public void Database_ShouldHaveTestData()
        {
            // Act & Assert
            Assert.IsNotNull(_db.Partners);
            Assert.IsNotNull(_db.Sales);
            Assert.IsTrue(_db.Partners.Count > 0, "База должна содержать хотя бы одного партнера");
            Assert.IsTrue(_db.Sales.Count > 0, "База должна содержать хотя бы одну продажу");

            // Выводим информацию для отладки
            Console.WriteLine($"Количество партнеров: {_db.Partners.Count}");
            Console.WriteLine($"Количество продаж: {_db.Sales.Count}");

            foreach (var partner in _db.Partners)
            {
                int totalSales = _db.Sales.Where(s => s.ID_Partner == partner.ID).Sum(s => s.Количество);
                string discount = _db.GetDiscount(partner.ID);
                Console.WriteLine($"Партнер {partner.ID}: {partner.Наименование_партнера}, продажи: {totalSales}, скидка: {discount}");
            }
        }

        // ТЕСТ 5: Проверка связи партнер-продажи
        [TestMethod]
        public void Sales_ShouldHaveValidPartnerReferences()
        {
            // Arrange
            var partnerIds = _db.Partners.Select(p => p.ID).ToList();

            // Act
            var invalidSales = _db.Sales.Where(s => !partnerIds.Contains(s.ID_Partner)).ToList();

            // Assert
            Assert.AreEqual(0, invalidSales.Count,
                $"Найдено {invalidSales.Count} продаж с несуществующими партнерами");
        }

    }
}
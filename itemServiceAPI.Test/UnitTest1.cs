using NUnit.Framework;
using Moq;
using ItemServiceAPI.Models;
using ItemServiceAPI.Controllers;
using ItemServiceAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace UnitTestController.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private Mock<IItemDbRepository> _itemDbRepositoryMock;
        private Mock<ILogger<ItemController>> _loggerMock;
        private ItemController _itemController;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<ItemController>>();
            _itemDbRepositoryMock = new Mock<IItemDbRepository>();
            _itemController = new ItemController(_loggerMock.Object, _itemDbRepositoryMock.Object);
        }

        // GetItem
        [Test]
        public async Task GetItem_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            var itemId = "item_123";
            var testItem = new Item { Id = itemId, Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.GetItemById(itemId))
                                 .ReturnsAsync(testItem); //Returnerer testItem, da item findes

            // Act
            var result = await _itemController.GetItem(itemId);

            // Assert
            
            //Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.That(result, Is.InstanceOf<OkObjectResult>()); //Ny versions måde at skrive på...

            var okResult = result as OkObjectResult;
            //Assert.IsNotNull(okResult);
            Assert.That(okResult.Value, Is.Not.Null);
            var returnedItem = okResult.Value as Item;
            //Assert.IsNotNull(returnedItem);
            Assert.That(returnedItem, Is.Not.Null);
            //Assert.AreEqual(itemId, returnedItem.Id);
            Assert.That(returnedItem.Id, Is.EqualTo(itemId));
        }

        // GetItem
        [Test]
        public async Task GetItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = "non_existing_item";

            _itemDbRepositoryMock.Setup(repo => repo.GetItemById(itemId))
                                 .ReturnsAsync((Item)null); //Returnerer null, da item ikke findes

            // Act
            var result = await _itemController.GetItem(itemId);

            // Assert
            //Assert.IsInstanceOf<NotFoundResult>(result);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
        
        
        // GetAllItems
        // Test på at der returneres en liste 
        [Test]
        public async Task GetAllItems_ShouldReturnListOfItems_WhenItemsExist()
        {
            // Arrange
            // Her oprettes testdata, 3 items
            var testItems = new List<Item>
            {
                new Item { Id = "item_001", Title = "Test Item 1" },
                new Item { Id = "item_002", Title = "Test Item 2" },
                new Item { Id = "item_003", Title = "Test Item 3" }
            };

            // De 3 items bliver her oprettet i mock DB
            _itemDbRepositoryMock.Setup(repo => repo.GetAllItems())
                                .ReturnsAsync(testItems); //Returnerer de 3 items

            // Act
            // Henter all items
            var result = await _itemController.GetAllItems();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Item>>(okResult.Value);

            // yderligere tjek på at det er de rigtige items der bliver fundet i listen
            var returnedItems = okResult.Value as List<Item>;
            Assert.AreEqual(3, returnedItems.Count);
            Assert.AreEqual("item_001", returnedItems[0].Id);
            Assert.AreEqual("item_002", returnedItems[1].Id);
            Assert.AreEqual("item_003", returnedItems[2].Id);
        }

        // GetAllItems
        // Test der tjekker at liste bliver returnet korrekt
        [Test]
        public async Task GetAllItems_ShouldReturnEmptyListCorrectly() //Måske skal denne ændres til at håndtere en null liste
        {
            // Arrange
            var testItems = new List<Item>(); // Tom liste

            _itemDbRepositoryMock.Setup(repo => repo.GetAllItems())
                                .ReturnsAsync(testItems); //Returnerer en tom liste

            // Act
            var result = await _itemController.GetAllItems();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult); // Tjek, at resultatet ikke er null
            Assert.IsInstanceOf<List<Item>>(okResult.Value); // Tjek, at returneret værdi er en liste af items

            var returnedItems = okResult.Value as List<Item>;
            Assert.IsNotNull(returnedItems); // Tjek, at listen ikke er null
            Assert.IsEmpty(returnedItems); // Tjek, at listen er tom
        }

        // GetAuctionableItems
        // Test, at der returneres en liste af items, som er auctionable
        [Test]
        public async Task GetAuctionableItems_ShouldReturnListOfItems_BasedOnDate()
        {
            // Arrange
            var auctionStart = DateTime.Now.AddDays(-2);
            var auctionEnd = DateTime.Now.AddDays(2);
            var testItems = new List<Item>
            {
                new Item { Id = "item_001", Title = "Test Item 1", StartAuctionDateTime = DateTimeOffset.Now.AddDays(-1), EndAuctionDateTime = DateTimeOffset.Now.AddDays(1) },
                new Item { Id = "item_002", Title = "Test Item 2", StartAuctionDateTime = DateTimeOffset.Now.AddDays(-1), EndAuctionDateTime = DateTimeOffset.Now.AddDays(1) },
                new Item { Id = "item_003", Title = "Test Item 3", StartAuctionDateTime = DateTimeOffset.Now.AddDays(-1), EndAuctionDateTime = DateTimeOffset.Now.AddDays(1) },
                new Item { Id = "item_004", Title = "Test Item 4", StartAuctionDateTime = DateTimeOffset.Now.AddDays(-7), EndAuctionDateTime = DateTimeOffset.Now.AddDays(-5) } //Denne skal gerne ikke med i listen over auctionable items
            };

            _itemDbRepositoryMock.Setup(repo => repo.GetAuctionableItems(auctionStart, auctionEnd))
                                .ReturnsAsync(testItems); //Returnere items [0,1,2] som er auctionable

            // Act
            var result = await _itemController.GetAuctionableItems(auctionStart, auctionEnd);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Item>>(okResult.Value);

            var returnedItems = okResult.Value as List<Item>;
            Assert.AreEqual(3, returnedItems.Count);
            Assert.AreEqual("item_001", returnedItems[0].Id);
            Assert.AreEqual("item_002", returnedItems[1].Id);
            Assert.AreEqual("item_003", returnedItems[2].Id);
            Assert.IsFalse(returnedItems.Exists(i => i.Id == "item_004")); //Denne skal ikke være i listen
        }

        // CreateItem
        //Test, at et gyldigt item kan oprettes (returnerer f.eks. 201 Created).
        [Test]
        public async Task CreateItem_ShouldReturnStatus201Created_WhenItemIsValid()
        {
            // Arrange
            var testItem = new Item { Id = "item_123", Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.CreateItem(testItem))
                                 .ReturnsAsync(true); //Returnerer true, da item er oprettet

            // Act
            var result = await _itemController.CreateItem(testItem);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result); //Tjek at der er returneret en CreatedAtActionResult
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode); //Tjek at statuskoden er 201 Created
        }

        // CreateItem
        //Test, at et ugyldigt item ikke kan oprettes (returnerer f.eks. 400 Bad Request).
        [Test]
        public async Task CreateItem_ShouldReturnStatus400BadRequest_WhenItemIsInvalid()
        {
            // Arrange
            var testItem = new Item { Id = "item_123", Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.CreateItem(testItem))
                                 .ReturnsAsync(false); //Returnerer false, da item ikke kan oprettes

            // Act
            var result = await _itemController.CreateItem(testItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result); //Tjek at der er returneret en BadRequestResult
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode); //Tjek at statuskoden er 400 Bad Request
        }

        // CreateItem
        // Test, at et item ikke kan oprettes, hvis id allerede findes (returnerer f.eks. 409 Conflict).
        [Test]
        public async Task CreateItem_ShouldReturnStatus409Conflict_WhenItemAlreadyExists()
        {
            // Arrange
            var testItem = new Item { Id = "item_123", Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.CreateItem(testItem))
                                 .ReturnsAsync(false); //Returnerer false, da item allerede findes

            // Act
            var result = await _itemController.CreateItem(testItem);

            // Assert
            Assert.IsInstanceOf<ConflictResult>(result); //Tjek at der er returneret en ConflictResult
            var conflictResult = result as ConflictResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual(409, conflictResult.StatusCode); //Tjek at statuskoden er 409 Conflict
        }

        // DeleteItem
        // Test, at et item kan slettes (returnerer f.eks. 204 No Content).
        [Test]
        public async Task DeleteItem_ShouldReturnStatus204NoContent_WhenItemIsDeleted()
        {
            // Arrange
            var itemId = "item_123";

            _itemDbRepositoryMock.Setup(repo => repo.DeleteItem(itemId))
                                 .ReturnsAsync(true);  //Returnerer true, da item er slettet

            // Act
            var result = await _itemController.DeleteItem(itemId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result); //Tjek at der er returneret en NoContentResult
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode); //Tjek at statuskoden er 204 No Content
        }

        // DeleteItem
        // Test, at et item ikke kan slettes, hvis id ikke findes (returnerer f.eks. 404 Not Found).
        [Test]
        public async Task DeleteItem_ShouldReturnStatus404NotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = "non_existing_item";

            _itemDbRepositoryMock.Setup(repo => repo.DeleteItem(itemId))
                                 .ReturnsAsync(false); //Returnerer false, da item ikke findes

            // Act
            var result = await _itemController.DeleteItem(itemId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result); //Tjek at der er returneret en NotFoundResult
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); //Tjek at statuskoden er 404 Not Found
        }

        // UpdateItem
        // Test, at et item kan opdateres (returnerer f.eks. 200 OK).
        [Test]
        public async Task UpdateItem_ShouldReturnStatus200OK_WhenItemIsUpdated()
        {
            // Arrange
            var itemId = "item_123";
            var testItem = new Item { Id = itemId, Title = "Updated Test Item" }; // Simuleret opdateret data

            // Mock repository: Antag at opdateringen lykkes
            _itemDbRepositoryMock.Setup(repo => repo.UpdateItem(itemId, testItem))
                                .ReturnsAsync(true); //Returnerer true, da item er opdateret

            // Act
            var result = await _itemController.UpdateItem(itemId, testItem);

            // Assert
            // 1. Tjek HTTP-responsen
            Assert.IsInstanceOf<OkResult>(result);
            var okResult = result as OkResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // 2. Verificér at repository-metoden blev kaldt korrekt
            _itemDbRepositoryMock.Verify(repo => repo.UpdateItem(itemId, testItem), Times.Once); //Tjek at UpdateItem blev kaldt én gang
        }


        // UpdateItem
        // Test, at et item ikke kan opdateres, hvis id ikke findes (returnerer f.eks. 404 Not Found).
        [Test]
        public async Task UpdateItem_ShouldReturnStatus404NotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = "non_existing_item";
            var testItem = new Item { Id = itemId, Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.UpdateItem(itemId, testItem))
                                 .ReturnsAsync(false); //Returnerer false, da item ikke findes

            // Act
            var result = await _itemController.UpdateItem(itemId, testItem);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result); //Tjek at der er returneret en NotFoundResult
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); //Tjek at statuskoden er 404 Not Found
        }

        // UpdateItem
        // Test, at et item ikke kan opdateres, hvis der mangler data (returnerer f.eks. 400 Bad Request).
        [Test]
        public async Task UpdateItem_ShouldReturnStatus400BadRequest_WhenItemIsInvalid()
        {
            // Arrange
            var itemId = "item_123";
            var testItem = new Item { Id = itemId, Title = null }; // Simuleret ugyldig data

            // Act
            var result = await _itemController.UpdateItem(itemId, testItem);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result); //Tjek at der er returneret en BadRequestResult
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode); //Tjek at statuskoden er 400 Bad Request
        }

        // GetItemsByOwnerId
        // Test, at der returneres en liste af items, som ejes af en bestemt bruger
        [Test]
        public async Task GetItemsByOwnerId_ShouldReturnListOfItems_BasedOnOwnerId()
        {
            // Arrange
            var ownerId = "user_123";
            var testItems = new List<Item>
            {
                new Item { Id = "item_001", Title = "Test Item 1", OwnerId = ownerId },
                new Item { Id = "item_002", Title = "Test Item 2", OwnerId = ownerId },
                new Item { Id = "item_003", Title = "Test Item 3", OwnerId = ownerId },
                new Item { Id = "item_004", Title = "Test Item 4", OwnerId = "user_456" } //Denne skal ikke med i listen over items
            };

            _itemDbRepositoryMock.Setup(repo => repo.GetItemsByOwnerId(ownerId))
                                .ReturnsAsync(testItems); //Returnere items [0,1,2] som ejes af brugeren

            // Act
            var result = await _itemController.GetItemsByOwnerId(ownerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Item>>(okResult.Value);

            var returnedItems = okResult.Value as List<Item>;
            Assert.AreEqual(3, returnedItems.Count);
            Assert.AreEqual("item_001", returnedItems[0].Id);
            Assert.AreEqual("item_002", returnedItems[1].Id);
            Assert.AreEqual("item_003", returnedItems[2].Id);
            Assert.IsFalse(returnedItems.Exists(i => i.Id == "item_004")); //Denne skal ikke være i listen
        }

        // GetItemsByOwnerId
        // Test, at der returneres en tom liste, hvis brugeren ikke ejer nogen items
        [Test]
        public async Task GetItemsByOwnerId_ShouldReturnEmptyList_WhenUserDoesNotOwnAnyItems()
        {
            // Arrange
            var ownerId = "user_123";
            var testItems = new List<Item>(); // Tom liste

            _itemDbRepositoryMock.Setup(repo => repo.GetItemsByOwnerId(ownerId))
                                .ReturnsAsync(testItems); //Returnerer en tom liste

            // Act
            var result = await _itemController.GetItemsByOwnerId(ownerId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Item>>(okResult.Value);

            var returnedItems = okResult.Value as List<Item>;
            Assert.IsNotNull(returnedItems);
            Assert.IsEmpty(returnedItems);
        }

        // GetItemsByOwnerId
        // Test, at der returneres en NotFound, hvis ownerId ikke findes
        [Test]
        public async Task GetItemsByOwnerId_ShouldReturnNotFound_WhenOwnerIdDoesNotExist()
        {
            // Arrange
            var ownerId = "non_existing_user";

            _itemDbRepositoryMock.Setup(repo => repo.GetItemsByOwnerId(ownerId))
                                .ReturnsAsync((List<Item>)null); //Returnerer null, da ownerId ikke findes

            // Act
            var result = await _itemController.GetItemsByOwnerId(ownerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result); //Tjek at der er returneret en NotFoundResult
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode); //Tjek at statuskoden er 404 Not Found
        }

        // Arrange

        // Act

        // Assert
    }
}
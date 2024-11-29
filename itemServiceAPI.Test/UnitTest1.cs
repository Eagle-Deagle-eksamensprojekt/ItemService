using NUnit.Framework;
using Moq;
using ItemServiceAPI.Models;
using ItemServiceAPI.Controllers;
using ItemServiceAPI.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        [Test]
        public async Task GetItem_ShouldReturnOk_WhenItemExists()
        {
            // Arrange
            var itemId = "item_123";
            var testItem = new Item { Id = itemId, Title = "Test Item" };

            _itemDbRepositoryMock.Setup(repo => repo.GetItemById(itemId))
                                 .ReturnsAsync(testItem);

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

        [Test]
        public async Task GetItem_ShouldReturnNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = "non_existing_item";

            _itemDbRepositoryMock.Setup(repo => repo.GetItemById(itemId))
                                 .ReturnsAsync((Item)null);

            // Act
            var result = await _itemController.GetItem(itemId);

            // Assert
            //Assert.IsInstanceOf<NotFoundResult>(result);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
        
        
        /// GetAllItems
        
        /// Test på at der returneres en liste, hvis items findes 
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
                                .ReturnsAsync(testItems);

            // Act
            // Henter all items
            var result = await _itemController.GetAllItems();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<List<Item>>(okResult.Value);

            var returnedItems = okResult.Value as List<Item>;
            Assert.AreEqual(3, returnedItems.Count);
            Assert.AreEqual("item_001", returnedItems[0].Id);
            Assert.AreEqual("item_002", returnedItems[1].Id);
            Assert.AreEqual("item_003", returnedItems[2].Id);
        }

        // Arrange

        // Act

        // Assert
    }
}
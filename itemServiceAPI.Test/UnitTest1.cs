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
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItem = okResult.Value as Item;
            Assert.IsNotNull(returnedItem);
            Assert.AreEqual(itemId, returnedItem.Id);
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
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetAllItems_ShouldReturnOk_WhenItemsExist()
        {
            // Arrange
            var testItems = new Item[]
            {
                new Item { Id = "item_1", Title = "Test Item 1" },
                new Item { Id = "item_2", Title = "Test Item 2" },
                new Item { Id = "item_3", Title = "Test Item 3" }
            };

            var testItemList = new List<Item>(testItems);
            _itemDbRepositoryMock.Setup(repo => repo.GetAllItems())
                                 .ReturnsAsync(testItemList);

            // Act
            var result = await _itemController.GetAllItems();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItems = okResult.Value as Item[];
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(testItems.Length, returnedItems.Length);
        }
    }
}

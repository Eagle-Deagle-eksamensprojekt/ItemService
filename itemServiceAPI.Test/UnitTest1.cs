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
    }
}
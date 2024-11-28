using NUnit.Framework;
using Moq;
using ItemServiceAPI.Models;
using ItemServiceAPI.Controllers;
using ItemServiceAPI;
using System;

namespace ItemService.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        private Mock<IItemRepository> _itemRepositoryMock;
        private ItemService _itemService;

        [SetUp]
        public void SetUp()
        {
            _itemRepositoryMock = new Mock<IItemRepository>();
            _itemService = new ItemService(_itemRepositoryMock.Object);
        }

        [Test]
        public void CreateItem_ShouldSaveItemToDatabase_WhenValidInput()
        {
            // Arrange
            var newItem = new Item
            {
                Title = "Test Item",
                Description = "Test Description",
                AuctionDate = DateTime.UtcNow.AddDays(1)
            };

            _itemRepositoryMock
                .Setup(repo => repo.CreateItem(newItem))
                .ReturnsAsync(true);

            // Act
            var result = _itemService.CreateItem(newItem);

            // Assert
            Assert.IsTrue(result.Result);
            _itemRepositoryMock.Verify(repo => repo.CreateItem(newItem), Times.Once);
        }

        [Test]
        public async Task GetItem_ShouldReturnItem_WhenIdIsValid()
        {
            // Arrange
            var itemId = "item_123";
            var testItem = new Item
            {
                Id = itemId,
                Title = "Test Item",
                Description = "Test Description",
                StartAuctionDateTime = DateTime.UtcNow.AddDays(1),
                EndAuctionDateTime = DateTime.UtcNow.AddDays(2)
            };

            _itemServiceMock.Setup(service => service.GetItemById(itemId))
                            .ReturnsAsync(testItem);

            // Act
            var result = await _controller.GetItem(itemId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedItem = okResult.Value as Item;
            Assert.IsNotNull(returnedItem);
            Assert.AreEqual(testItem.Id, returnedItem.Id);
        }

    }

    
}

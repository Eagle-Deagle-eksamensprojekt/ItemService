using MongoDB.Driver;
using ItemServiceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ItemServiceAPI.Services;

namespace Services
{
    public class ItemMongoDBService : IItemDbRepository
    {
        private readonly IMongoCollection<Item> _itemCollection;
        private readonly ILogger<ItemMongoDBService> _logger;

        public ItemMongoDBService(ILogger<ItemMongoDBService> logger, IConfiguration configuration)
        {
            _logger = logger;

            var connectionString = configuration["MongoConnectionString"] ?? "<blank>";
            var databaseName = configuration["DatabaseName"] ?? "<blank>";
            var collectionName = configuration["CollectionName"] ?? "<blank>";
            
            _logger.LogInformation($"Connecting to MongoDB using: {connectionString}");
            _logger.LogInformation($"Using database: {databaseName}");
            _logger.LogInformation($"Using collection: {collectionName}");

            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _itemCollection = database.GetCollection<Item>(collectionName);
                _logger.LogInformation("Connected to MongoDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
                throw; 
            }
        }

        public async Task<bool> CreateItem(Item item)
        {
            try 
            {
                await _itemCollection.InsertOneAsync(item); // Insert the item
                _logger.LogInformation("Item created: {0}", item);
                return true; // Return true if the item was created
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating item: {0}", ex.Message);
                throw;
            }
        }
        public Task<Item> GetItemById(string id) // ændre it til itemId i hele repository
        {
            try
            {
                var item = _itemCollection.Find(i => i.Id == id).FirstOrDefault(); // Find the item
                _logger.LogInformation("Item found: {0}", item);
                return Task.FromResult(item); // Return the item
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching item: {0}", ex.Message);
                throw;
            }
        }
        public Task<List<Item>> GetAllItems()
        {
            try
            {
                var items = _itemCollection.Find(i => true).ToList(); // Find all items
                _logger.LogInformation($"{items.Count} items found.");
                return Task.FromResult(items); // Return the items
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching all items: {0}", ex.Message);
                throw;
            }
        }
        public Task<bool> DeleteItem(string id)
        {
            try
            {
                var result = _itemCollection.DeleteOne(i => i.Id == id); // Delete the item
                _logger.LogInformation("Deleted item: {0}", id);
                return Task.FromResult(result.DeletedCount == 1); // Return true if one item was deleted    
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting item: {0}", ex.Message);
                throw; // Rethrow the exception
            }
        }
        public Task<List<Item>> GetAuctionableItems(DateTime currentDateTime)
        {
            try
            {
                var auctionableItems = _itemCollection.Find(i => i.StartAuctionDateTime <= currentDateTime && i.EndAuctionDateTime >= currentDateTime).ToList(); // Find all items where the auction is active
                _logger.LogInformation($"{auctionableItems.Count} auctionable items found.");
                return Task.FromResult(auctionableItems); // Return the auctionable items
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching auctionable items: {0}", ex.Message);
                throw;
            }
        }

        public Task<List<Item>> GetItemsByOwnerId(string ownerId)
        {
            try
            {
                var items = _itemCollection.Find(i => i.OwnerId == ownerId).ToList(); // Find all items with the specified ownerId
                _logger.LogInformation($"{items.Count} items found for owner: {0}", ownerId);
                return Task.FromResult(items);  // Return the items
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching items for owner: {0}", ex.Message);
                throw;  // Rethrow the exception
            }
        }
        public Task<bool> UpdateItem(Item item)
        {
            try
            {
                var result = _itemCollection.ReplaceOne(i => i.Id == item.Id, item); // Replace the item
                _logger.LogInformation("Updated item: {0}", item);
                return Task.FromResult(result.ModifiedCount == 1); // Return true if one item was modified
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating item: {0}", ex.Message);
                throw;
            }
        }

        public async Task<bool> CheckItemIsAuctionable(string id, DateTime currentDateTime)
        {
            try
            {
                var item = await _itemCollection.Find(i => i.Id == id).FirstOrDefaultAsync(); // Asynchronous query
                if (item == null)
                {
                    _logger.LogWarning("Item with ID {0} not found.", id);
                    return false; // Not auctionable if item doesn't exist
                }

                var isAuctionable = item.StartAuctionDateTime <= currentDateTime && item.EndAuctionDateTime >= currentDateTime;
                _logger.LogInformation("Item {0} is auctionable: {1}", id, isAuctionable);
                return isAuctionable;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking if item is auctionable: {0}", ex.Message);
                throw;
            }
        }
    }
}
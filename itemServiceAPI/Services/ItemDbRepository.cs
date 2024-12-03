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
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to connect to MongoDB: {0}", ex.Message);
                throw;
            }
        }

        /*
        public async Task<Item> CreateUserAsync(Item item)
        {
            // TODO: Add additional validation if needed
            try
            {
                await _itemCollection.InsertOneAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating user: {0}", ex.Message);
                throw;
            }
        }*/
        public async Task<bool> CreateItem(Item item)
        {
            try 
            {
                await _itemCollection.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating item: {0}", ex.Message);
                throw;
            }
        }
        public Task<Item> GetItemById(string id)
        {
            try
            {
                var item = _itemCollection.Find(i => i.Id == id).FirstOrDefault();
                return Task.FromResult(item);
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
                var items = _itemCollection.Find(i => true).ToList();
                return Task.FromResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching all items: {0}", ex.Message);
                throw;
            }
        }
        public Task<bool> DeleteItem(string id)
        {
            throw new NotImplementedException();
        }
        public Task<List<Item>> GetAuctionableItems(DateTime currentDateTime)
        {
            throw new NotImplementedException();
        }

        public Task<List<Item>> GetItemsByOwnerId(string ownerId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> UpdateItem(Item item)
        {
            throw new NotImplementedException();
        }

    }
}
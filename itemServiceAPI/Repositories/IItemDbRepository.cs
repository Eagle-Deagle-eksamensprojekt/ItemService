using ItemServiceAPI.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ItemServiceAPI.Repositories
{
    public interface IItemDbRepository
    {
        Task<bool> CreateItem(Item item);
        Task<Item> GetItemById(string id);
        Task<List<Item>> GetAllItems();
        Task<bool> UpdateItem(string id, Item updatedItem);
        Task<bool> DeleteItem(string id);
        Task<List<Item>> GetAuctionableItems(DateTime auctionStart, DateTime auctionEnd);
        Task<object> GetItemsByOwnerId(string ownerId); // Endten skal det være denne eller den næste
        //Task<List<Item>> GetItemsByOwner(string ownerId); // Skal dette være en liste???
        
    }
}
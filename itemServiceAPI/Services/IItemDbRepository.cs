using ItemServiceAPI.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ItemServiceAPI.Services
{
    public interface IItemDbRepository
    {
        Task<bool> CreateItem(Item item);
        Task<Item> GetItemById(string id);
        Task<List<Item>> GetAllItems();
        Task<bool> DeleteItem(string id);
        Task<List<Item>> GetAuctionableItems(DateTime currentDateTime); 

        Task<List<Item>> GetItemsByOwnerId(string ownerId); // Endten skal det være denne eller den næste
        Task<bool> UpdateItem(Item item); // Hvorfor skal jeg have 2 UpdateItem metoder?

    }
}
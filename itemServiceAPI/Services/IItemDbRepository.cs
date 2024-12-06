using ItemServiceAPI.Models;


namespace ItemServiceAPI.Services
{
    public interface IItemDbRepository
    {
        Task<bool> CreateItem(Item item);
        Task<Item> GetItemById(string id); // ændre id til itemId i hele solution
        Task<List<Item>> GetAllItems();
        Task<bool> DeleteItem(string id);
        Task<List<Item>> GetAuctionableItems(DateTime currentDateTime); 

        Task<List<Item>> GetItemsByOwnerId(string ownerId); // Endten skal det være denne eller den næste
        Task<bool> UpdateItem(Item item); // Hvorfor skal jeg have 2 UpdateItem metoder?

    }
}
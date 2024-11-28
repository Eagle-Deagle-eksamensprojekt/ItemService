namespace ItemServiceAPI.Repositories
{
    public interface IItemRepository
    {
        Task<bool> CreateItem(Item item);
        Task<Item> GetItemById(string id);
        Task<List<Item>> GetAllItems();
        Task<bool> UpdateItem(string id, Item updatedItem);
        Task<bool> DeleteItem(string id);
    }
}
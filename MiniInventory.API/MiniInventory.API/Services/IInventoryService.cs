using MiniInventory.API.Models;

namespace MiniInventory.API.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync();
        Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction);
    }
}
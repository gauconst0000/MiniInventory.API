using MiniInventory.API.Models;
using System.IO;
using System.Threading.Tasks;

namespace MiniInventory.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<byte[]> GenerateExcelTemplateAsync();
        Task<string> ImportProductsFromExcelAsync(Stream excelStream);
        Task<byte[]> ExportAllProductsToExcelAsync();
    }
}
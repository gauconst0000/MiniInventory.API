using MiniInventory.API.Models;

namespace MiniInventory.API.Services
{
    // Chữ 'interface' thay vì 'class'
    public interface IProductService
    {
        // Khai báo món số 1: Thêm sản phẩm (Chỉ ghi tên, không ghi cách làm)
        Task<Product> AddProductAsync(Product product);
    }
}
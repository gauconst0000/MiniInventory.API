using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;

namespace MiniInventory.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // 2. Lấy chi tiết
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        // 3. Thêm mới
        public async Task<Product> AddProductAsync(Product product)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);
            if (!categoryExists)
            {
                throw new Exception("Loại sản phẩm (CategoryId) không tồn tại!");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        // 4. Cập nhật
        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // 5. Xóa
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return;

            // 1. Kiểm tra xem sản phẩm đã có lịch sử giao dịch chưa
            var hasTransactions = await _context.InventoryTransactionDetails
                                                .AnyAsync(t => t.ProductId == id);

            if (hasTransactions)
            {
                // 2. Nếu ĐÃ CÓ giao dịch -> Chuyển trạng thái
                product.Status = "INACTIVE";
                _context.Entry(product).State = EntityState.Modified;
            }
            else
            {
                // 3. Nếu CHƯA CÓ giao dịch -> Cho phép xóa vật lý
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;
// Em nhớ using thêm thư mục chứa AppDbContext và Product của em vào đây nhé

namespace MiniInventory.API.Services
{
    // Đầu bếp ProductService ký hợp đồng cung cấp các món trong IProductService
    public class ProductService : IProductService
    {
        // Kho nguyên liệu (Database)
        private readonly ApplicationDbContext _context;

        // Dependency Injection: Tiêm kho nguyên liệu vào cho Đầu bếp khi nhận việc
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Bắt đầu xào nấu món "Thêm sản phẩm"
        public async Task<Product> AddProductAsync(Product product)
        {
            // 1. CHUYỂN LOGIC VÀO ĐÂY: Kiểm tra xem CategoryId có tồn tại không
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId);

            if (!categoryExists)
            {
                // Nếu sai logic, quăng lỗi ra để Lễ tân báo lại cho khách
                throw new Exception("Loại sản phẩm (CategoryId) không tồn tại!");
            }

            // 2. Thêm vào kho và lưu lại
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product; // Trả về món hàng đã hoàn thiện
        }
    }
}
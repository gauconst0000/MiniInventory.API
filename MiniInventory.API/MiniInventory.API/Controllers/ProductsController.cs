using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;
using MiniInventory.API.Services;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Chỉ những người đã đăng nhập (có Token) mới được vào đây
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // 1. Khai báo thêm Đầu bếp
        private readonly IProductService _productService;
        // 2. Tiêm Đầu bếp vào (Dependency Injection)
        public ProductsController(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // 1. Lấy danh sách sản phẩm (kèm theo thông tin Loại sản phẩm)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // 2. Lấy chi tiết 1 sản phẩm theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound("Không tìm thấy sản phẩm!");
            return product;
        }

        // 3. Thêm mới sản phẩm
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            try
            {
                // Lễ tân giao việc cho Đầu bếp (Service)
                var newProduct = await _productService.AddProductAsync(product);

                // Trả kết quả cho khách (Angular)
                return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
            }
            catch (Exception ex)
            {
                // Báo lỗi 400 BadRequest nếu Đầu bếp phát hiện sai sót (ví dụ: sai CategoryId)
                return BadRequest(ex.Message);
            }
        }

        // 4. Cập nhật thông tin sản phẩm
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id) return BadRequest();

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. Xóa sản phẩm
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
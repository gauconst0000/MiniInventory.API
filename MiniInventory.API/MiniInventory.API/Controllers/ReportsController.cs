using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Vẫn cần thẻ từ để bảo mật thông tin kinh doanh
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. API cảnh báo hàng sắp hết (Tồn kho nhỏ hơn 10)
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStockProducts()
        {
            var products = await _context.Products
                .Where(p => p.StockQuantity < 10)
                .ToListAsync();

            return Ok(products);
        }

        // 2. API tính tổng giá trị kho hàng
        [HttpGet("inventory-value")]
        public async Task<ActionResult> GetTotalInventoryValue()
        {
            var totalValue = await _context.Products
                .SumAsync(p => p.StockQuantity * p.PurchasePrice);

            return Ok(new
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalInventoryValue = totalValue
            });
        }
    }
}
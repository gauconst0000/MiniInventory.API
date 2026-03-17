using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InventoryTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách phiếu nhập/xuất (kèm chi tiết)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetTransactions()
        {
            return await _context.InventoryTransactions
                .Include(t => t.TransactionDetails)
                .ToListAsync();
        }

        // 2. Tạo phiếu Nhập hoặc Xuất kho (Có xử lý tăng/giảm tồn kho)
        [HttpPost]
        public async Task<ActionResult<InventoryTransaction>> PostTransaction(InventoryTransaction transaction)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.InventoryTransactions.Add(transaction);

                foreach (var detail in transaction.TransactionDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null) return BadRequest($"Sản phẩm ID {detail.ProductId} không tồn tại!");

                    // Logic xử lý tồn kho
                    if (transaction.TransactionType == "Inbound") // Nhập kho
                    {
                        product.StockQuantity += detail.Quantity;
                    }
                    else if (transaction.TransactionType == "Outbound") // Xuất kho
                    {
                        if (product.StockQuantity < detail.Quantity)
                            return BadRequest($"Sản phẩm {product.Name} không đủ hàng để xuất!");

                        product.StockQuantity -= detail.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}
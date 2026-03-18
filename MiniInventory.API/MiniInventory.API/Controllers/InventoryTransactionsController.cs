using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniInventory.API.Models;
using MiniInventory.API.Services;

namespace MiniInventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryTransactionsController : ControllerBase
    {
        // TIẾP TỤC TUYỆT ĐỐI KHÔNG GỌI DBCONTEXT
        private readonly IInventoryService _inventoryService;

        public InventoryTransactionsController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetTransactions()
        {
            var transactions = await _inventoryService.GetTransactionsAsync();
            return Ok(transactions);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryTransaction>> PostTransaction(InventoryTransaction transaction)
        {
            try
            {
                // Giao việc khó cho Đầu bếp
                var newTransaction = await _inventoryService.CreateTransactionAsync(transaction);
                return Ok(newTransaction);
            }
            catch (Exception ex)
            {
                // Nếu Đầu bếp báo nguyên liệu hỏng (hết hàng, sai ID), báo ngay cho khách
                return BadRequest(ex.Message);
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;
using MiniInventory.API.Data;
using MiniInventory.API.Models;

namespace MiniInventory.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync()
        {
            // 🚀 Lời giải của sếp Tùng: Bỏ Include, tự viết LINQ Select để Join dữ liệu
            var transactions = await _context.InventoryTransactions
                .AsNoTracking()
                .Select(t => new InventoryTransaction
                {
                    Id = t.Id,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    ContactPerson = t.ContactPerson,
                    Notes = t.Notes,

                    // 1. Tự móc sang bảng Details (chi tiết phiếu)
                    TransactionDetails = _context.InventoryTransactionDetails
                        .Where(td => td.TransactionId == t.Id)
                        .Select(td => new InventoryTransactionDetail
                        {
                            Id = td.Id,
                            TransactionId = td.TransactionId,
                            ProductId = td.ProductId,
                            Quantity = td.Quantity,
                            UnitPrice = td.UnitPrice,

                            // 🚀 2. Tự Join sang bảng Product bằng tay để lấy tên!
                            Product = _context.Products.FirstOrDefault(p => p.Id == td.ProductId)
                        }).ToList()
                })
                .ToListAsync();

            return transactions;
        }

        public async Task<InventoryTransaction> CreateTransactionAsync(InventoryTransaction transaction)
        {
            // Mở khóa an toàn: Đảm bảo dữ liệu chỉ lưu khi tất cả đều đúng
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.InventoryTransactions.Add(transaction);

                foreach (var detail in transaction.TransactionDetails)
                {
                    var product = await _context.Products.FindAsync(detail.ProductId);
                    if (product == null)
                        throw new Exception($"Sản phẩm ID {detail.ProductId} không tồn tại!");

                    // Logic xử lý tồn kho
                    if (transaction.TransactionType == "Inbound") // Nhập kho
                    {
                        product.StockQuantity += detail.Quantity;
                    }
                    else if (transaction.TransactionType == "Outbound") // Xuất kho
                    {
                        if (product.StockQuantity < detail.Quantity)
                            throw new Exception($"Sản phẩm {product.Name} không đủ hàng để xuất!");

                        product.StockQuantity -= detail.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync(); // Xác nhận lưu thành công

                return transaction;
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync(); // Hủy bỏ mọi thay đổi nếu có lỗi
                throw; // Ném lỗi ra ngoài cho Controller xử lý
            }
        }
    }
}
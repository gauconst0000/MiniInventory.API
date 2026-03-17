namespace MiniInventory.API.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string TransactionType { get; set; } = string.Empty; // "Inbound" (Nhập) hoặc "Outbound" (Xuất)
        public string ContactPerson { get; set; } = string.Empty; // Người giao hoặc Người nhận
        public string? Notes { get; set; }

        // Mối quan hệ: Một phiếu sẽ chứa nhiều dòng chi tiết sản phẩm
        public ICollection<InventoryTransactionDetail> TransactionDetails { get; set; } = new List<InventoryTransactionDetail>();
    }
}
using System.Text.Json.Serialization; // <-- Thêm dòng này
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniInventory.API.Models
{
    public class InventoryTransactionDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int TransactionId { get; set; }

        [JsonIgnore] // <-- Thêm cái này để Swagger không bắt nhập
        public InventoryTransaction? Transaction { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
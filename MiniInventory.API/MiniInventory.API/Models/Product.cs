using System.Text.Json.Serialization; // <-- Thêm dòng này
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniInventory.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int StockQuantity { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore] // <-- Thêm cái này
        public Category? Category { get; set; }
    }
}
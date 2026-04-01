using System;
using System.ComponentModel.DataAnnotations;

namespace MiniInventory.API.Models // Chú ý đổi namespace cho khớp với dự án của em nhé
{
    public class SystemLog
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; } // Người dùng nào đang thao tác (nếu có)

        [Required]
        public string Method { get; set; } = string.Empty; // GET, POST, PUT, DELETE...

        [Required]
        public string Endpoint { get; set; } = string.Empty; // API nào được gọi (VD: /api/Products)

        public int StatusCode { get; set; } // Mã kết quả (200 là ngon, 400/500 là lỗi)

        public string? ErrorMessage { get; set; } // Nếu có lỗi thì ghi chi tiết vào đây

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian ghi hình (Cực kỳ quan trọng để lát nữa làm Cronjob xóa log quá hạn)
    }
}
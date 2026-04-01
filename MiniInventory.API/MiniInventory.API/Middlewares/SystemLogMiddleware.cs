using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MiniInventory.API.Models; // Đổi lại namespace Models của em nhé
using MiniInventory.API.Data; // Thêm thư mục chứa DbContext của em vào đây

namespace MiniInventory.API.Middlewares
{
    public class SystemLogMiddleware
    {
        private readonly RequestDelegate _next;

        public SystemLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // DbContext phải được inject vào đây (InvokeAsync) để không bị lỗi vòng đời (lifecycle)
        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            var log = new SystemLog
            {
                // Lấy tên người dùng nếu họ đã đăng nhập (có vé Token)
                Username = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Khách vãng lai",
                Method = context.Request.Method,
                Endpoint = context.Request.Path,
                CreatedAt = DateTime.Now
            };

            try
            {
                // Cho phép Request đi tiếp vào các API bên trong (Nhập, Xuất kho...)
                await _next(context);

                // Sau khi API chạy xong, ghi lại kết quả (Thành công 200, hay lỗi 400, 401...)
                log.StatusCode = context.Response.StatusCode;
            }
            catch (Exception ex)
            {
                // Nếu API bên trong bị nổ (Code lỗi sập server), tóm ngay cái lỗi đó lại!
                log.StatusCode = 500;
                log.ErrorMessage = ex.Message;
                throw; // Vẫn quăng lỗi ra ngoài để hệ thống biết, nhưng mình đã kịp ghi hình rồi
            }
            finally
            {
                // Cuối cùng, lưu cuộn băng ghi hình vào Database
                dbContext.SystemLogs.Add(log);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
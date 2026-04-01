using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniInventory.API.Data; // Đổi lại thành namespace chứa ApplicationDbContext của em
using MiniInventory.API.Models; // Đổi lại namespace Models

namespace MiniInventory.API.Services
{
    // Kế thừa BackgroundService để nó thành tác vụ chạy ngầm
    public class LogCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LogCleanupService> _logger;

        public LogCleanupService(IServiceProvider serviceProvider, ILogger<LogCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Vòng lặp vô tận: Cứ chạy xong là ngủ, đến giờ lại dậy chạy tiếp
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Cô lao công dọn log bắt đầu làm việc lúc: {time}", DateTimeOffset.Now);

                try
                {
                    // Vì BackgroundService chạy liên tục, ta phải tạo một "phạm vi" (Scope) riêng để gọi Database
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // TODO: Tạm thời Hardcode 30 ngày. Về sau có thể làm API để Admin tự chỉnh con số 30 này lưu vào DB.
                        int daysToKeep = 30;
                        var expirationDate = DateTime.Now.AddDays(-daysToKeep);

                        // Quét tìm tất cả các log cũ hơn expirationDate
                        var oldLogs = dbContext.SystemLogs.Where(l => l.CreatedAt < expirationDate).ToList();

                        if (oldLogs.Any())
                        {
                            dbContext.SystemLogs.RemoveRange(oldLogs); // Hốt rác
                            await dbContext.SaveChangesAsync(stoppingToken); // Đổ rác
                            _logger.LogInformation($"Đã dọn dẹp thành công {oldLogs.Count} bản ghi log quá hạn.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy Cronjob dọn rác log.");
                }

                // Cô lao công đi ngủ 24 tiếng (1 ngày). 
                // Mẹo: Trong lúc test em có thể đổi thành TimeSpan.FromMinutes(1) để 1 phút nó dọn 1 lần xem có chạy không nhé.
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
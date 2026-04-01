using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Bổ sung thư viện đọc Config
using Microsoft.Extensions.DependencyInjection;
using MiniInventory.API.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace MiniInventory.API.Services
{
    public class LogCleanupService : BackgroundService
    {
        private readonly ILogger<LogCleanupService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration; // Bổ sung biến Config

        // Tiêm IConfiguration vào Lễ tân
        public LogCleanupService(ILogger<LogCleanupService> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("... Cô lao công dọn log bắt đầu làm việc lúc: {time} ...", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        // 1. ĐỌC CẤU HÌNH NGÀY LƯU TỪ APPSETTINGS.JSON
                        // Nếu trong appsettings không ghi, sẽ lấy mặc định là 30 ngày
                        int retentionDays = _configuration.GetValue<int>("LogCleanupSettings:RetentionDays", 30);

                        // Tính toán ngày hết hạn dựa trên số ngày vừa đọc được
                        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

                        _logger.LogWarning("... Đang chuẩn bị dọn dẹp các bản ghi log cũ hơn {days} ngày (trước: {date}) ...", retentionDays, cutoffDate);

                        var oldLogs = context.SystemLogs.Where(log => log.CreatedAt < cutoffDate).ToList();

                        if (oldLogs.Any())
                        {
                            context.SystemLogs.RemoveRange(oldLogs);
                            await context.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("... Đã dọn dẹp thành công [{count}] bản ghi log quá hạn. ...", oldLogs.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy cronjob dọn dẹp log.");
                }

                // 2. CHỈNH CRONJOB: Ngủ đông 1 ngày (24 tiếng) rồi mới dậy dọn tiếp
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
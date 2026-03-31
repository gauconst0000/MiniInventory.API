using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiniInventory.API.Services
{
    // Kế thừa BackgroundService để biến class này thành một Tác vụ chạy ngầm
    public class InventoryCheckJob : BackgroundService
    {
        private readonly ILogger<InventoryCheckJob> _logger;

        public InventoryCheckJob(ILogger<InventoryCheckJob> logger)
        {
            _logger = logger;
        }

        // Lõi của hệ thống ngầm: Hàm này sẽ tự động chạy ngay khi Web bật lên
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("⏳ [CRONJOB] Bảo vệ ca đêm đã vào ca!");

            // Vòng lặp chạy liên tục cho đến khi tắt server
            while (!stoppingToken.IsCancellationRequested)
            {
                // Hành động đi tuần: Ghi vào nhật ký
                _logger.LogInformation("🔍 [CRONJOB] Đang tự động đi tuần tra kho hàng lúc: {time}", DateTimeOffset.Now);

                // Đi tuần xong, ngồi nghỉ 10 giây rồi mới chạy vòng lặp tiếp
                // (Thực tế người ta hay để 24 giờ, 1 tuần... nhưng mình để 10 giây để test cho lẹ)
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
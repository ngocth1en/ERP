using ERP.Domain.Enum.ASM;
using ERP.Infrastructure.ERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Infrastructure.Services
{
    public class HandoverExpiryService : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);
        private static readonly int PendingLimitDays = 3;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HandoverExpiryService> _logger;

        public HandoverExpiryService(
            IServiceScopeFactory scopeFactory,
            ILogger<HandoverExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CancelExpiredAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tự động hủy phiếu bàn giao quá hạn");
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task CancelExpiredAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Phiếu Pending được tạo cách đây > 3 ngày
            var cutoff = DateTime.Now.AddDays(-PendingLimitDays);

            var expired = await context.Asset_Handovers
                .Where(h => h.Status == HandoverStatus.Pending
                         && h.CreatedAt < cutoff)
                .ToListAsync();

            if (!expired.Any()) return;

            foreach (var h in expired)
            {
                h.Status = HandoverStatus.Cancelled;
                h.UpdatedAt = DateTime.Now;
            }

            await context.SaveChangesAsync();

            _logger.LogInformation(
                "Tự động hủy {Count} phiếu bàn giao quá hạn 3 ngày (trước {Cutoff:dd/MM/yyyy})",
                expired.Count, cutoff);
        }
    }
}

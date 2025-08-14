using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RdlcSignalRDemo.Data;
using RdlcSignalRDemo.Hubs;
using RdlcSignalRDemo.Models;

namespace RdlcSignalRDemo.Services
{
    public class BackgroundReportService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IHubContext<ReportHub> _hubContext;

        public BackgroundReportService(IServiceProvider services, IHubContext<ReportHub> hubContext)
        {
            _services = services;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var generator = scope.ServiceProvider.GetRequiredService<ReportGeneratorService>();

                // Get first pending job
                var job = await db.ReportJobs
                    .Where(j => j.Status == JobStatus.Pending)
                    .FirstOrDefaultAsync(stoppingToken);

                if (job != null)
                {
                    try
                    {
                        job.Status = JobStatus.InProgress;
                        job.Progress = 10;
                        await db.SaveChangesAsync(stoppingToken);
                        await SendUpdate(job);

                        await Task.Delay(500, stoppingToken);
                        job.Progress = 40;
                        await db.SaveChangesAsync(stoppingToken);
                        await SendUpdate(job);

                        string fileName = await generator.GeneratePdfAsync(job.ReportName);
                        job.FilePath = fileName;
                        job.Progress = 100;
                        job.Status = JobStatus.Completed;
                        await db.SaveChangesAsync(stoppingToken);
                        await SendUpdate(job);
                    }
                    catch
                    {
                        job.Status = JobStatus.Failed;
                        await db.SaveChangesAsync(stoppingToken);
                        await SendUpdate(job);
                    }
                }
                else
                {
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
        private async Task SendUpdate(ReportJob job)
        {
            await _hubContext.Clients.Group(job.Id.ToString())
                .SendAsync("UpdateJobStatus", job.Id, job.Status.ToString(), job.Progress, job.FilePath);
        }
    }
}

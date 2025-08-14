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

                // Get all pending jobs
                var jobs = await db.ReportJobs
                    .Where(j => j.Status == JobStatus.Pending)
                    .OrderBy(j => j.CreatedUtc)
                    .ToListAsync(stoppingToken);

                // Start all pending jobs in parallel
                var tasks = jobs.Select(job => ProcessJobAsync(job, db, generator, stoppingToken)).ToList();

                if (tasks.Count > 0)
                    await Task.WhenAll(tasks);

                await Task.Delay(500, stoppingToken); // small delay to reduce CPU usage
            }
        }

        private async Task ProcessJobAsync(ReportJob job, AppDbContext db, ReportGeneratorService generator, CancellationToken token)
        {
            try
            {
                job.Status = JobStatus.InProgress;
                job.Progress = 10;
                await db.SaveChangesAsync(token);
                await SendUpdate(job);

                await Task.Delay(200, token);
                job.Progress = 40;
                await db.SaveChangesAsync(token);
                await SendUpdate(job);

                // Generate PDF in parallel (thread-safe, unique file per job)
                string fileName = await generator.GeneratePdfAsync(job.ReportName);
                job.FilePath = fileName;
                job.Progress = 100;
                job.Status = JobStatus.Completed;
                await db.SaveChangesAsync(token);
                await SendUpdate(job);
            }
            catch
            {
                job.Status = JobStatus.Failed;
                await db.SaveChangesAsync(token);
                await SendUpdate(job);
            }
        }
        private async Task SendUpdate(ReportJob job)
        {
            await _hubContext.Clients.Group(job.Id.ToString())
                .SendAsync("UpdateJobStatus", job.Id, job.Status.ToString(), job.Progress, job.FilePath);
        }
    }
}

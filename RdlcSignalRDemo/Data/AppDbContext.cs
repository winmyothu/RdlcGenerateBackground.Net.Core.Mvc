using Microsoft.EntityFrameworkCore;
using RdlcSignalRDemo.Models;

namespace RdlcSignalRDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ReportJob> ReportJobs => Set<ReportJob>();
}

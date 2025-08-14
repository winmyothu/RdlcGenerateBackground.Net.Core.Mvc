using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RdlcSignalRDemo.Data;
using RdlcSignalRDemo.Models;
using RdlcSignalRDemo.Services;

namespace RdlcSignalRDemo.Controllers;

public class ReportsController : Controller
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db; 
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> List()
    {
        var jobs = await _db.ReportJobs
            .OrderByDescending(j => j.CreatedUtc)
            .ToListAsync();
        return View(jobs);
    }

    [HttpPost]
    public async Task<IActionResult> Enqueue(string reportName = "Sample")
    {
        var job = new ReportJob
        {
            ReportName = reportName
        };
        _db.ReportJobs.Add(job);
        await _db.SaveChangesAsync();
        return Json(new { jobId = job.Id });
    }
}

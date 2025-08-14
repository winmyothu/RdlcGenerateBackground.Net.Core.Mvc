namespace RdlcSignalRDemo.Models;

public enum ReportJobStatus { Pending, Queued, Processing, Completed, Failed }
public enum JobStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
}


public class ReportJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReportName { get; set; } = default!;
    public string? ParametersJson { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Pending;
    public int Progress { get; set; } = 0;

    public string? FilePath { get; set; }
    public string? Error { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedUtc { get; set; }
}

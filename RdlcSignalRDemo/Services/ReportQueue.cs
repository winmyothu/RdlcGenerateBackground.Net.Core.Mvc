using System.Collections.Concurrent;
using RdlcSignalRDemo.Models;

namespace RdlcSignalRDemo.Services
{
    public class ReportQueue
    {

        private ConcurrentQueue<ReportJob> _jobs = new ConcurrentQueue<ReportJob>();
        public void Enqueue(ReportJob job) => _jobs.Enqueue(job);
        public bool TryDequeue(out ReportJob job) => _jobs.TryDequeue(out job);

    }
}

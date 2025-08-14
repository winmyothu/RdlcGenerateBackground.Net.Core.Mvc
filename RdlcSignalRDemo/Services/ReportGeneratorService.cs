using System.Data;
using Microsoft.Reporting.NETCore;

namespace RdlcSignalRDemo.Services
{
    public class ReportGeneratorService
    {
        private readonly string _reportFolder;

        public ReportGeneratorService(IWebHostEnvironment env)
        {
            _reportFolder = Path.Combine(env.ContentRootPath, "Reports");
        }

        public async Task<string> GeneratePdfAsync(string templateName)
        {
            var reportPath = Path.Combine(_reportFolder, templateName+".rdlc");

            LocalReport report = new LocalReport();
            report.ReportPath = reportPath;
            var fileName = $"{Guid.NewGuid()}.pdf";
            var outputPath = $"/reports/Output/{fileName}";


            // Example DataTable (replace with your actual data source)
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Example");
            //dt.Rows.Add("Dynamic Data without parameters");

            //report.DataSources.Add(new ReportDataSource("DataSet1", dt));

            var pdf = report.Render("PDF");

            var outputFolder = Path.Combine(_reportFolder, "Output");
            Directory.CreateDirectory(outputFolder);

            var filePath = Path.Combine(outputFolder, fileName);
            await File.WriteAllBytesAsync(filePath, pdf);
            
            return outputPath;
        }
    }
}

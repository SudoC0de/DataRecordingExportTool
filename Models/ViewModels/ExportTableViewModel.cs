using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class ExportTableViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }

        public void Cleanup()
        {
            TableName = null;
            TableNames = null;
        }
    }
}

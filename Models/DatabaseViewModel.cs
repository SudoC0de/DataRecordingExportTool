using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models
{
    public class DatabaseViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }
    }
}

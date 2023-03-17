using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models
{
    public class ModifyTableViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }
        public string? TableTask { get; set; }
        public SelectList? TableTasks { get; set; }
    }
}

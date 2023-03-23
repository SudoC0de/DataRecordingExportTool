using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class ModifyDataStep1ViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }
        public string? TableTask { get; set; }
        public SelectList? TableTasks { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class DisplayTableStep1ViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }
    }
}

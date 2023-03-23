using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class DeleteViewModel
    {
        public string? TableName { get; set; }
        public SelectList? TableNames { get; set; }
    }
}

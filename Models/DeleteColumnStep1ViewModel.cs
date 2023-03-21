using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models
{
    public class DeleteColumnStep1ViewModel
    {
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
        public SelectList? ColumnNames { get; set; }
    }
}

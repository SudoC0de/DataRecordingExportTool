using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class DeleteColumnStep1ViewModel
    {
        public string? TableName { get; set; }
        public string? ColumnName { get; set; }
        public SelectList? ColumnNames { get; set; }

        public void Cleanup()
        {
            TableName = null;
            ColumnName = null;
            ColumnNames = null;
        }
    }
}

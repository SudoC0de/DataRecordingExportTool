using DataRecordingExportTool.Models.Objects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class DisplayTableStep2ViewModel
    {
        public Table? Table { get; set; }
        public List<Row>? Rows { get; set; }

        public void Cleanup()
        {
            Table = null;
            Rows.Clear();
            Rows = null;
        }
    }
}

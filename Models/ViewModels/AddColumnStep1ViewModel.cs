using DataRecordingExportTool.Models.Objects;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class AddColumnStep1ViewModel
    {
        public Table? Table { get; set; }
        public Column? Column { get; set; }

        public void Cleanup()
        {
            Table.Name = string.Empty;
            Table.NumberOfColumns = 0;

            foreach(Column c in Table.Columns)
            {
                c.Name = string.Empty;
            }

            Table.Columns.Clear();
            Table = null;

            Column.Name = string.Empty;
            Column = null;
        }
    }
}

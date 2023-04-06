using DataRecordingExportTool.Models.Objects;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class RemoveDataRowStep1ViewModel
    {
        public Table? Table { get; set; }
        public List<Row>? Rows { get; set; }
        public int DeleteRowId { get; set; } = 0;

        public void Cleanup()
        {
            if (Table != null)
            {
                Table.Name = string.Empty;
                Table.NumberOfColumns = 0;

                foreach (Column column in Table.Columns)
                {
                    column.Name = string.Empty;
                }

                Table = null;
            }

            if (Rows != null)
            {
                foreach (Row row in Rows)
                {
                    row.Id = 0;
                    
                    foreach (DataPoint d in row.DataPoints)
                    {
                        d.Data = string.Empty;
                    }

                    foreach (Column c in row.Columns)
                    {
                        c.Name = string.Empty;
                    }
                }

                Rows = null;
            }

            DeleteRowId = 0;
        }
    }
}

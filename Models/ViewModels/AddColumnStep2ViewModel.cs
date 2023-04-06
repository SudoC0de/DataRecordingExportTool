using DataRecordingExportTool.Models.Objects;

namespace DataRecordingExportTool.Models.ViewModels
{
    public class AddColumnStep2ViewModel
    {
        public Table? Table { get; set; }
        public List<Row>? TableRows { get; set; }

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

            foreach(Row r in TableRows)
            {
                r.Id = 0;
                
                foreach (DataPoint d in r.DataPoints)
                {
                    d.Data = string.Empty;
                }

                r.DataPoints.Clear();

                foreach (Column c in r.Columns)
                {
                    c.Name = string.Empty;
                }

                r.Columns.Clear();
            }

            TableRows.Clear();
            TableRows = null;
        }
    }
}

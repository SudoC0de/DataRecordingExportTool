namespace DataRecordingExportTool.Models
{
    public class Row
    {
        public List<DataPoint> DataPoints { get; set; } = new List<DataPoint>(0);
        public List<Column> Columns { get; set; } = new List<Column>(0);
    }
}

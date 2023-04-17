namespace DataRecordingExportTool.Models.Objects
{
    public class Row
    {
        public int Id { get; set; }
        public List<DataPoint> DataPoints { get; set; } = new List<DataPoint>(0);
        public List<Column> Columns { get; set; } = new List<Column>(0);
    }
}

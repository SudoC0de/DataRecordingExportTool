namespace DataRecordingExportTool.Models;

public static class ObjectPool
{
    public static Dictionary<string, Table> Tables = new Dictionary<string, Table>(0);
    public static DeleteViewModel? DeleteViewModel { get; set; } = null;
    public static ModifyDataStep1ViewModel? ModifyDataStep1ViewModel { get; set; } = null;
    public static AddDataStep1ViewModel? AddDataStep1ViewModel { get; set; } = null;
    public static DeleteColumnStep1ViewModel? DeleteColumnStep1ViewModel { get; set; } = null;
}

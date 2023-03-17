using Microsoft.AspNetCore.Mvc;

namespace DataRecordingExportTool.Models;

public static class ObjectPool
{
    public static Dictionary<string, Table> Tables = new Dictionary<string, Table>(0);
    public static Dictionary<string, DatabaseViewModel> Databases = new Dictionary<string, DatabaseViewModel>(0);
    public static Dictionary<string, ModifyTableViewModel> TableModifications = new Dictionary<string, ModifyTableViewModel>(0);
    public static List<string> Messages = new List<string>(0);
}

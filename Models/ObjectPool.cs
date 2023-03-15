using Microsoft.AspNetCore.Mvc;

namespace DataRecordingExportTool.Models;

public static class ObjectPool
{
    public static Dictionary<string, Table> Tables = new Dictionary<string, Table>(0);
}

using Microsoft.AspNetCore.Mvc;

namespace DataRecordingExportTool.Models;

public static class ObjectPool
{
    public static Table CurTable = new Table();
}

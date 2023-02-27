using Microsoft.AspNetCore.Mvc;

namespace DataRecordingExportTool.Controllers;

public class ExportController : Controller
{
    // GET: /Export/
    public IActionResult Index()
    {
        return View();
    }
}

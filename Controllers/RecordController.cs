using Microsoft.AspNetCore.Mvc;

namespace DataRecordingExportTool.Controllers;

public class RecordController : Controller
{
    // GET: /Record/
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult CreateTable()
    {
        return View("CreateTable");
    }

    public IActionResult RecordData()
    {
        return View("RecordData");
    }

    [HttpPost, ActionName("CreateResultsTable")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateResultsTable()
    {
        return View("CreateTable");
    }
}

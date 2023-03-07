using Microsoft.AspNetCore.Mvc;
using DataRecordingExportTool.Models;

namespace DataRecordingExportTool.Controllers;

public class RecordController : Controller
{
    // GET: /Record/
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet, ActionName("CreateTableStep1")]
    public IActionResult CreateTableStep1()
    {
        // List<Column> columnsCollection = new List<Column>(0);

        // return View(columnsCollection);
        return View("CreateTableStep1");
    }

    [HttpGet]
    public IActionResult RecordData()
    {
        return View("RecordData");
    }

    [HttpPost, ActionName("GoToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToCreateResultTableStep2([Bind("Name,NumberOfColumns")]Table table)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateTableStep1", table);
        }

        return RedirectToAction("CreateTableStep2");
    }
}

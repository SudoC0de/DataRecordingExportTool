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

    [HttpGet]
    public IActionResult CreateTable()
    {
        return View("CreateTable");
    }

    [HttpGet]
    public IActionResult RecordData()
    {
        return View("RecordData");
    }

    [HttpPost, ActionName("CreateResultTable")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateResultTable()
    {
        if (!ModelState.IsValid)
        {
            return View("CreateTable");
        }

        return View("CreateTable");
    }
}

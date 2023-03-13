using Microsoft.AspNetCore.Mvc;
using DataRecordingExportTool.Models;
using System.Xml;
using System.Xml.XPath;

namespace DataRecordingExportTool.Controllers;

public class RecordController : Controller
{
    private readonly string _webEnvironmentRootDirectory;

    public RecordController(IWebHostEnvironment environment)
    {
        _webEnvironmentRootDirectory = environment.WebRootPath;
    }

    // GET: /Record/
    public IActionResult Index()
    {
        return View("RecordShortcuts");
    }

    #region HttpGet

    #region Create Table
    [HttpGet, ActionName("CreateTableStep1")]
    public IActionResult CreateTableStep1()
    {
        return View("CreateTableStep1");
    }

    [HttpGet, ActionName("CreateTableStep2")]
    public IActionResult CreateTableStep2()
    {
        return View("CreateTableStep2", ObjectPool.CurTable);
    }

    [HttpGet, ActionName("CreateTableStep3")]
    public IActionResult CreateTableStep3()
    {
        return View("RecordShortcuts");
    }
    #endregion
    
    #region Record Data Points
    [HttpGet]
    public IActionResult RecordData()
    {
        return View("RecordData");
    }
    #endregion
    
    #endregion

    #region HttpPost

    #region Create Table
    [HttpPost, ActionName("GoToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToCreateResultTableStep2([Bind("Name,NumberOfColumns")]Table table)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateTableStep1", table);
        }

        // Check for duplicate table names here


        ObjectPool.CurTable.Name = table.Name;
        ObjectPool.CurTable.NumberOfColumns = table.NumberOfColumns;

        for (int i = 0; i < ObjectPool.CurTable.NumberOfColumns; i++)
        {
            ObjectPool.CurTable.Columns.Add(new Column());
        }

        return RedirectToAction("CreateTableStep2");
    }

    [HttpPost, ActionName("GoToStep3")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToCreateResultTableStep3(Table table)
    {
        //Check for duplicate columns here
        for (int i = 0; i < table.Columns.Count; i++)
        {
            ObjectPool.CurTable.Columns[i].Name = table.Columns[i].Name;
        }

        AddTableToXmlDb();
        ObjectPool.CurTable = new Table();

        return RedirectToAction("CreateTableStep3");
    }

    private void AddTableToXmlDb()
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNode rooteNode = xml.DocumentElement;

        if (rooteNode != null)
        {
            XmlElement tableElement = xml.CreateElement("table");
            XmlAttribute nameAttri = xml.CreateAttribute("name");

            nameAttri.Value = ObjectPool.CurTable.Name;
            tableElement.Attributes.Append(nameAttri);

            XmlElement headerRowElement = xml.CreateElement("header");
            
            foreach (Column column in ObjectPool.CurTable.Columns)
            {
                XmlElement columnElement = xml.CreateElement("Column");
                XmlAttribute columnName = xml.CreateAttribute("name");
                
                columnName.Value = column.Name;
                columnElement.Attributes.Append(columnName);
                headerRowElement.AppendChild(columnElement);
            }

            tableElement.AppendChild(headerRowElement);
            rooteNode.AppendChild(tableElement);
            xml.Save(databasePath);
        }  
    }
    #endregion
    
    #endregion
}

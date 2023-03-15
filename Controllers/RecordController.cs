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
        return View("CreateTableStep2", ObjectPool.Tables["TableCreate"]);
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
        if (DoesTableExist(table.Name))
        {
            return View("CreateTableStep1", table);
        }
        else
        {
            ObjectPool.Tables.Add("TableCreate", new Table()
            {
                Name = table.Name,
                NumberOfColumns = table.NumberOfColumns
            });

            for (int i = 0; i < ObjectPool.Tables["TableCreate"].NumberOfColumns; i++)
            {
                ObjectPool.Tables["TableCreate"]
                          .Columns
                          .Add(new Column());
            }

            return RedirectToAction("CreateTableStep2");
        }
    }

    [HttpPost, ActionName("GoToStep3")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToCreateResultTableStep3(Table table)
    {
        for (int i = 0; i < table.Columns.Count; i++)
        {
            ObjectPool.Tables["TableCreate"]
                      .Columns[i]
                      .Name = table.Columns[i]
                                   .Name;
        }

        //Check for duplicate columns here
        if (DoDuplicateColumnsExist(ObjectPool.Tables["TableCreate"].Columns))
        {
            return RedirectToAction("CreateTableStep2");
        }
        else
        {
            AddTableToXmlDb();
            ObjectPool.Tables["TableCreate"] = new Table();
            ObjectPool.Tables.Remove("TableCreate");

            return RedirectToAction("CreateTableStep3");
        }
    }

    #endregion

    #region Helpers
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

            nameAttri.Value = ObjectPool.Tables["TableCreate"].Name;
            tableElement.Attributes.Append(nameAttri);

            XmlElement headerRowElement = xml.CreateElement("header");

            foreach (Column column in ObjectPool.Tables["TableCreate"].Columns)
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

    private bool DoesTableExist(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNode rooteNode = xml.DocumentElement;

        if (rooteNode != null)
        {
            XmlNode? node = rooteNode.SelectSingleNode($"//database/table[@name='{tableName}']");

            if (node != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool DoDuplicateColumnsExist(List<Column> columns)
    {
        List<string> columnNames = new List<string>(0);

        foreach (Column column in columns)
        {
            columnNames.Add(column.Name);
        }

        foreach (Column column in columns)
        {
            int nameCount = columnNames.Where(name => name == column.Name)
                                       .Count();

            if (nameCount > 1)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #endregion
}

using Microsoft.AspNetCore.Mvc;
using DataRecordingExportTool.Models;
using System.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DataRecordingExportTool.Controllers;

public class RecordController : Controller
{
    private readonly string _webEnvironmentRootDirectory;

    public RecordController(IWebHostEnvironment environment)
    {
        _webEnvironmentRootDirectory = environment.WebRootPath;
    }

    #region HttpGet

    public IActionResult Index()
    {
        if (ObjectPool.Databases.ContainsKey("DeleteTable"))
        {
            ObjectPool.Databases["DeleteTable"] = new DatabaseViewModel();
            ObjectPool.Databases.Remove("DeleteTable");
        }

        if (ObjectPool.Tables.ContainsKey("TableCreate"))
        {
            ObjectPool.Tables["TableCreate"] = new Table();
        }

        return View("RecordShortcuts");
    }

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

    #region Delete Table
    [HttpGet, ActionName("DeleteTableStep1")]
    public IActionResult DeleteTableStep1()
    {
        if (ObjectPool.Databases.ContainsKey("DeleteTable") == false)
        {
            ObjectPool.Databases.Add("DeleteTable", new DatabaseViewModel
                                                    {
                                                        TableNames = new SelectList(GetTableNames())
                                                    });
        }

        return View("DeleteTableStep1", ObjectPool.Databases["DeleteTable"]);
    }

    [HttpGet, ActionName("DeleteTableStep2")]
    public IActionResult DeleteTableStep2()
    {
        return View("RecordShortcuts");
    }
    #endregion

    #region Record Data Points
    [HttpGet, ActionName("RecordDataStep1")]
    public IActionResult RecordData()
    {
        if (ObjectPool.TableModifications.ContainsKey("ModifyTable") == false)
        {
            ObjectPool.TableModifications.Add("ModifyTable", new ModifyTableViewModel
                                                             {
                                                                 TableNames = new SelectList(GetTableNames()),
                                                                 TableTasks = new SelectList(new List<string>(2)
                                                                 {
                                                                     "Add Data Point",
                                                                     "Delete Table Column"
                                                                 })
                                                             });
        }

        return View("RecordDataStep1", ObjectPool.TableModifications["ModifyTable"]);
    }

    [HttpGet, ActionName("AddDataPointStep1")]
    public IActionResult AddDataPointStep1()
    {
        return View("RecordShortcuts");
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

    #region Delete Table
    [HttpPost, ActionName("DeleteTableGoToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToDeleteResultTableStep2(DatabaseViewModel dbVM)
    {
        if (DeleteTable(dbVM.TableName))
        {
            ObjectPool.Databases["DeleteTable"] = new DatabaseViewModel();
            ObjectPool.Databases.Remove("DeleteTable");

            return RedirectToAction("DeleteTableStep2");
        }

        return RedirectToAction("DeleteTableStep1");
    }
    #endregion

    #region Modify Table
    [HttpPost, ActionName("ModifyTableGoToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToModifyResultTableStep2(ModifyTableViewModel mTVM)
    {
        ObjectPool.Tables.Add("Modify", new Table()
        {
            Name = mTVM.TableName,
            NumberOfColumns = GetNumTableColumns(mTVM.TableName),
            Columns = GetColumns(mTVM.TableName)
        });

        switch (mTVM.TableTask)
        {
            case "Add Data Point":
                {
                    break;
                }
            case "Delete Table Column":
                {
                    break;
                }
        }

        return RedirectToAction("AddDataPointStep1");
    }
    #endregion

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

    private List<string> GetTableNames()
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? nodes = xml.DocumentElement
                                .SelectNodes($"//database/table");

        if (nodes != null)
        {
            var tableNames = new List<string>(0);

            foreach (XmlNode node in nodes)
            {
                tableNames.Add(node.Attributes["name"].Value);
            }

            return tableNames;
        }

        return new List<string>(0);
    }

    private bool DeleteTable(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNode? node = xml.DocumentElement
                           .SelectSingleNode($"//database/table[@name='{tableName}']");

        if (node != null)
        {
            xml.DocumentElement.RemoveChild(node);
            xml.Save(databasePath);

            return true;
        }

        return false;
    }

    private int GetNumTableColumns(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        return xml.DocumentElement
                  .SelectNodes($"//database/table[@name='{tableName}']/header/Column")
                  .Count;
    }

    private List<Column> GetColumns(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? nodes = xml.DocumentElement
                                .SelectNodes($"//database/table[@name='{tableName}']/header/Column");

        List<Column> columns = new List<Column>(0);

        foreach(XmlNode column in nodes)
        {
            columns.Add(new Column()
            {
                Name = column.Attributes["name"].Value
            });
        }

        return columns;
    }
    #endregion
}

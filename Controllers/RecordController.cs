using Microsoft.AspNetCore.Mvc;
using DataRecordingExportTool.Models;
using System.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Eventing.Reader;
using System.ComponentModel.Design.Serialization;

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
        if (ObjectPool.DeleteViewModel != null)
        {
            ObjectPool.DeleteViewModel.TableName = null;
            ObjectPool.DeleteViewModel.TableNames = null;
            ObjectPool.DeleteViewModel = null;
        }
        else if (ObjectPool.ModifyDataStep1ViewModel != null)
        {
            ObjectPool.ModifyDataStep1ViewModel.TableName = null;
            ObjectPool.ModifyDataStep1ViewModel.TableNames = null;
            ObjectPool.ModifyDataStep1ViewModel.TableTask = null;
            ObjectPool.ModifyDataStep1ViewModel.TableTasks = null;
            ObjectPool.ModifyDataStep1ViewModel = null;
        }
        else if (ObjectPool.AddDataStep1ViewModel != null)
        {
            ObjectPool.AddDataStep1ViewModel.TableName = null;
            ObjectPool.AddDataStep1ViewModel.Row = null;
            ObjectPool.AddDataStep1ViewModel = null;
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
        if (ObjectPool.DeleteViewModel == null)
        {
            ObjectPool.DeleteViewModel = new DeleteViewModel
            {
                TableNames = new SelectList(GetTableNames())
            };
        }

        return View("DeleteTableStep1", ObjectPool.DeleteViewModel);
    }

    [HttpGet, ActionName("DeleteTableStep2")]
    public IActionResult DeleteTableStep2()
    {
        return View("RecordShortcuts");
    }
    #endregion

    #region Modify Table
    [HttpGet, ActionName("ModifyDataStep1")]
    public IActionResult ModifyDataStep1()
    {
        if (ObjectPool.ModifyDataStep1ViewModel ==  null)
        {
            ObjectPool.ModifyDataStep1ViewModel = new ModifyDataStep1ViewModel()
            {
                TableNames = new SelectList(GetTableNames()),
                TableTasks = new SelectList(new List<string>(2)
                                            {
                                                "Add Data Point",
                                                "Delete Table Column"
                                            })
            };
        }                                                     

        return View("ModifyDataStep1", ObjectPool.ModifyDataStep1ViewModel);
    }

    [HttpGet, ActionName("AddDataPointStep1")]
    public IActionResult AddDataPointStep1()
    {
        return View("AddDataStep1", ObjectPool.AddDataStep1ViewModel);
    }

    [HttpGet, ActionName("AddDataPointStep2")]
    public IActionResult AddDataPointStep2()
    {
        return View("RecordShortcuts");
    }

    [HttpGet, ActionName("DeleteColumnStep1")]
    public IActionResult DeleteColumnStep1()
    {
        return View("DeleteColumnStep1", ObjectPool.DeleteColumnStep1ViewModel);
    }

    [HttpGet, ActionName("DeleteColumnStep2")]
    public IActionResult DeleteColumnStep2()
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
    public IActionResult GoToDeleteResultTableStep2(DeleteViewModel dbVM)
    {
        if (DeleteTable(dbVM.TableName))
        {
            return RedirectToAction("DeleteTableStep2");
        }

        return RedirectToAction("DeleteTableStep1");
    }
    #endregion

    #region Modify Table
    [HttpPost, ActionName("ModifyTableGoToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToModifyResultTableStep2(ModifyDataStep1ViewModel mTVM)
    {
        switch (mTVM.TableTask)
        {
            case "Add Data Point":
                {
                    Row curRow = new Row()
                    {
                        Columns = GetColumns(mTVM.TableName)
                    };

                    for (int i = 0; i < curRow.Columns.Count; i++)
                    {
                        curRow.DataPoints.Add(new DataPoint());
                    }

                    ObjectPool.AddDataStep1ViewModel = new AddDataStep1ViewModel()
                    {
                        TableName = mTVM.TableName,
                        Row = curRow
                    };

                    return RedirectToAction("AddDataPointStep1");
                }
            case "Delete Table Column":
                {
                    ObjectPool.DeleteColumnStep1ViewModel = new DeleteColumnStep1ViewModel()
                    {
                        TableName = mTVM.TableName,
                        ColumnNames = new SelectList(GetColumnNames(mTVM.TableName))
                    };

                    return RedirectToAction("DeleteColumnStep1");
                }
            default:
                {
                    return RedirectToAction("ModifyDataStep1");
                }
        }
    }

    [HttpPost, ActionName("AddDataPoints")]
    [ValidateAntiForgeryToken]
    public IActionResult AddDataPoints(AddDataStep1ViewModel mTVM)
    {
        if (ModelState.IsValid)
        {
            for (int i = 0; i < mTVM.Row.DataPoints.Count; i++)
            {
                ObjectPool.AddDataStep1ViewModel.Row.DataPoints[i].Data = mTVM.Row.DataPoints[i].Data;
            }

            AddRowToXmlDb();

            return RedirectToAction("AddDataPointStep2");
        }

        return RedirectToAction("AddDataPointStep1");
    }

    [HttpPost, ActionName("DeleteTableColumn")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteTableColumn(DeleteColumnStep1ViewModel mTVM)
    {
        if (ModelState.IsValid)
        {
            ObjectPool.DeleteColumnStep1ViewModel.ColumnName = mTVM.ColumnName;
            DeleteColumnFromXmlDb();

            return RedirectToAction("DeleteColumnStep2");
        }

        return RedirectToAction("DeleteColumnStep1");
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

    private void AddRowToXmlDb()
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlElement rowElement = xml.CreateElement("Row");
        XmlAttribute idAttri = xml.CreateAttribute("id");

        idAttri.Value = (GetNumTableRows(ObjectPool.AddDataStep1ViewModel.TableName) + 1).ToString();
        rowElement.Attributes.Append(idAttri);

        for (int i = 0; i < ObjectPool.AddDataStep1ViewModel.Row.DataPoints.Count; i++)
        {
            XmlElement dataPoint = xml.CreateElement("DataPoint");
            XmlAttribute name = xml.CreateAttribute("name");
            XmlAttribute data = xml.CreateAttribute("data");

            name.Value = ObjectPool.AddDataStep1ViewModel.Row.Columns[i].Name;
            data.Value = ObjectPool.AddDataStep1ViewModel.Row.DataPoints[i].Data;
            dataPoint.Attributes.Append(name);
            dataPoint.Attributes.Append(data);
            rowElement.AppendChild(dataPoint);
        }

        xml.DocumentElement
           .SelectSingleNode($"//database/table[@name='{ObjectPool.AddDataStep1ViewModel.TableName}']")
           .AppendChild(rowElement);
        xml.Save(databasePath);
    }

    private void DeleteColumnFromXmlDb()
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? nodes = xml.DocumentElement
                                .SelectNodes($"//database/table[@name='{ObjectPool.DeleteColumnStep1ViewModel.TableName}']/header/Column");
        
        for (int i = 0; i < nodes.Count; i++)
        {
            if (string.Equals(nodes[i].Attributes["name"].Value, ObjectPool.DeleteColumnStep1ViewModel.ColumnName))
            {
                xml.DocumentElement
                   .SelectSingleNode($"//database/table[@name='{ObjectPool.DeleteColumnStep1ViewModel.TableName}']/header")
                   .RemoveChild(nodes[i]);
                break;
            }
        }

        nodes = xml.DocumentElement
                   .SelectNodes($"//database/table[@name='{ObjectPool.DeleteColumnStep1ViewModel.TableName}']/Row");

        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNodeList? dataPoints = xml.DocumentElement
                                         .SelectNodes($"//database/table[@name='{ObjectPool.DeleteColumnStep1ViewModel.TableName}']/Row[@id='{i + 1}']/DataPoint");

            for (int i2 = 0; i2 < dataPoints.Count; i2++)
            {
                if (string.Equals(dataPoints[i2].Attributes["name"].Value, ObjectPool.DeleteColumnStep1ViewModel.ColumnName))
                {
                    xml.DocumentElement
                       .SelectSingleNode($"//database/table[@name='{ObjectPool.DeleteColumnStep1ViewModel.TableName}']/Row[@id='{i + 1}']")
                       .RemoveChild(dataPoints[i2]);
                    break;
                }
            }
        }

        xml.Save(databasePath);
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

    private List <string> GetColumnNames(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? nodes = xml.DocumentElement
                                .SelectNodes($"//database/table[@name='{tableName}']/header/Column");
        List<string> columnNames = new List<string>(0);

        foreach(XmlNode column in nodes)
        {
            columnNames.Add(column.Attributes["name"].Value);
        }

        return columnNames;
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

    private int GetNumTableRows(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        return xml.DocumentElement
                  .SelectNodes($"//database/table[@name='{tableName}']/Row")
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

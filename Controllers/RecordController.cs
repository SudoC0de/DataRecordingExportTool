using Microsoft.AspNetCore.Mvc;
using System.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataRecordingExportTool.Models.ViewModels;
using DataRecordingExportTool.Models.Objects;

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
            ObjectPool.DeleteViewModel = null;
        }
        if (ObjectPool.ModifyDataStep1ViewModel != null)
        {
            ObjectPool.ModifyDataStep1ViewModel = null;
        }
        if (ObjectPool.AddDataStep1ViewModel != null)
        {
            ObjectPool.AddDataStep1ViewModel = null;
        }
        if (ObjectPool.DisplayTableStep1ViewModel != null)
        {
            ObjectPool.DisplayTableStep1ViewModel = null;
        }
        if (ObjectPool.DisplayTableStep2ViewModel != null)
        {
            ObjectPool.DisplayTableStep2ViewModel = null;
        }
        if (ObjectPool.RemoveDataRowStep1ViewModel != null)
        {
            ObjectPool.RemoveDataRowStep1ViewModel.Cleanup();
            ObjectPool.RemoveDataRowStep1ViewModel = null;
        }
        if (ObjectPool.AddColumnStep1ViewModel  != null)
        {
            ObjectPool.AddColumnStep1ViewModel.Cleanup();
            ObjectPool.AddColumnStep1ViewModel = null;
        }
        if (ObjectPool.AddColumnStep2ViewModel != null)
        {
            ObjectPool.AddColumnStep2ViewModel.Cleanup();
            ObjectPool.AddColumnStep2ViewModel = null;
        }
        if (ObjectPool.ExportTableViewModel != null)
        {
            ObjectPool.ExportTableViewModel.Cleanup();
            ObjectPool.ExportTableViewModel = null;
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
                                                "Add Data Row",
                                                "Remove Data Row",
                                                "Add Table Column",
                                                "Remove Table Column"
                                            })
            };
        }                                                     

        return View("ModifyDataStep1", ObjectPool.ModifyDataStep1ViewModel);
    }

    [HttpGet, ActionName("AddDataRowStep1")]
    public IActionResult AddDataRowStep1()
    {
        return View("AddDataStep1", ObjectPool.AddDataStep1ViewModel);
    }

    [HttpGet, ActionName("AddDataRowStep2")]
    public IActionResult AddDataRowStep2()
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

    [HttpGet, ActionName("RemoveDataRowStep1")]
    public IActionResult RemoveDataRowStep1()
    {
        return View("RemoveDataStep1", ObjectPool.RemoveDataRowStep1ViewModel);
    }

    [HttpGet, ActionName("RemoveDataRowStep2")]
    public IActionResult RemoveDataRowStep2()
    {
        return View("RecordShortcuts");
    }

    [HttpGet, ActionName("AddColumnStep1")]
    public IActionResult AddColumnStep1()
    {
        return View("AddColumnStep1", ObjectPool.AddColumnStep1ViewModel);
    }

    [HttpGet, ActionName("AddColumnStep2")]
    public IActionResult AddColumnStep2()
    {
        return View("AddColumnStep2", ObjectPool.AddColumnStep2ViewModel);
    }

    [HttpGet, ActionName("AddColumnStep3")]
    public IActionResult AddColumnStep3()
    {
        return View("RecordShortcuts");
    }
    #endregion

    #region Display Table
    [HttpGet, ActionName("DisplayTableStep1")]
    public IActionResult DisplayTableStep1()
    {
        if (ObjectPool.DisplayTableStep1ViewModel == null)
        {
            ObjectPool.DisplayTableStep1ViewModel = new DisplayTableStep1ViewModel()
            {
                TableNames = new SelectList(GetTableNames())
            };
        }

        return View("DisplayTableStep1", ObjectPool.DisplayTableStep1ViewModel);
    }

    [HttpGet, ActionName("DisplayTableStep2")]
    public IActionResult DisplayTableStep2()
    {
        return View("DisplayTableStep2", ObjectPool.DisplayTableStep2ViewModel);
    }

    [HttpGet, ActionName("DisplayTableStep3")]
    public IActionResult DisplayTableStep3()
    {
        return View("RecordShortcuts");
    }
    #endregion

    #region Export Table
    [HttpGet, ActionName("ExportTableStep1")]
    public IActionResult ExportTableStep1()
    {
        if (ObjectPool.ExportTableViewModel == null)
        {
            ObjectPool.ExportTableViewModel = new ExportTableViewModel()
            {
                TableNames = new SelectList(GetTableNames())
            };
        }

        return View("ExportTableStep1", ObjectPool.ExportTableViewModel);
    }

    [HttpGet, ActionName("ExportTableStep2")]
    public IActionResult ExportTableStep2()
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
            case "Add Data Row":
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

                    return RedirectToAction("AddDataRowStep1");
                }
            case "Remove Data Row":
                {
                    ObjectPool.RemoveDataRowStep1ViewModel = new RemoveDataRowStep1ViewModel()
                    {
                        Table = new Table()
                        {
                            Name = mTVM.TableName,
                            NumberOfColumns = GetColumns(mTVM.TableName).Count,
                            Columns = GetColumns(mTVM.TableName)
                        },
                        Rows = GetTableRows(mTVM.TableName),
                        DeleteRowId = 0
                    };

                    return RedirectToAction("RemoveDataRowStep1");
                }
            case "Add Table Column":
                {
                    ObjectPool.AddColumnStep1ViewModel = new AddColumnStep1ViewModel()
                    {
                        Table = new Table()
                        {
                            Name = mTVM.TableName,
                            NumberOfColumns = GetColumns(mTVM.TableName).Count,
                            Columns = GetColumns(mTVM.TableName)
                        },
                        Column = new Column()
                        {
                            Name = string.Empty
                        }
                    };

                    return RedirectToAction("AddColumnStep1");
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

            return RedirectToAction("AddDataRowStep2");
        }

        return RedirectToAction("AddDataRowStep1");
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

    [HttpPost, ActionName("DeleteTableRow")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteTableRow(RemoveDataRowStep1ViewModel rDVM)
    {
        if (ModelState.IsValid)
        {
            ObjectPool.RemoveDataRowStep1ViewModel.DeleteRowId = rDVM.DeleteRowId + 1;
            DeleteRowFromXmlDb(ObjectPool.RemoveDataRowStep1ViewModel.Table.Name);

            return RedirectToAction("RemoveDataRowStep2");
        }

        return RedirectToAction("RemoveDataRowStep1");
    }

    [HttpPost, ActionName("GoToAddColumnStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToAddColumnStep2(AddColumnStep1ViewModel aCVM)
    {
        if (ModelState.IsValid)
        {
            ObjectPool.AddColumnStep2ViewModel = new AddColumnStep2ViewModel()
            {
                Table = new Table()
                {
                    Name = aCVM.Table.Name,
                    NumberOfColumns = GetColumnNames(aCVM.Table.Name).Count + 1,
                    Columns = GetColumns(aCVM.Table.Name)
                },
                TableRows = GetTableRows(aCVM.Table.Name)
            };

            ObjectPool.AddColumnStep2ViewModel.Table.Columns.Add(aCVM.Column);

            for (int rowIndex = 0; rowIndex < ObjectPool.AddColumnStep2ViewModel.TableRows.Count; rowIndex++)
            {
                ObjectPool.AddColumnStep2ViewModel.TableRows[rowIndex].Columns.Add(aCVM.Column);
                ObjectPool.AddColumnStep2ViewModel.TableRows[rowIndex].DataPoints.Add(new DataPoint() { Data = "NA" });
            }

            ObjectPool.AddColumnStep1ViewModel.Cleanup();
            ObjectPool.AddColumnStep1ViewModel = null;

            return RedirectToAction("AddColumnStep2");
        }

        return RedirectToAction("AddColumnStep1");
    }

    [HttpPost, ActionName("GoToAddColumnStep3")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToAddColumnStep3(AddColumnStep2ViewModel aCVM)
    {
        for (int rowIndex = 0; rowIndex < ObjectPool.AddColumnStep2ViewModel.TableRows.Count; rowIndex++)
        {
            for (int rowColIndex = 0; rowColIndex < ObjectPool.AddColumnStep2ViewModel.TableRows[rowIndex].Columns.Count; rowColIndex++)
            {
                if (string.Equals(ObjectPool.AddColumnStep2ViewModel.TableRows[rowIndex].DataPoints[rowColIndex].Data, "NA"))
                {
                    ObjectPool.AddColumnStep2ViewModel.TableRows[rowIndex].DataPoints[rowColIndex].Data = aCVM.TableRows[rowIndex].DataPoints[rowColIndex].Data;
                }
            }
        }

        if (ObjectPool.AddColumnStep2ViewModel.TableRows.Any(r => r.DataPoints.Any(d => d.Data == "NA")))
        {
            return RedirectToAction("AddColumnStep2");
        }
        else
        {
            AddColumnToXmlDb();
            ObjectPool.AddColumnStep2ViewModel.Cleanup();
            ObjectPool.AddColumnStep2ViewModel = null;

            return RedirectToAction("AddColumnStep3");
        }
    }
    #endregion

    #region Display Table
    [HttpPost, ActionName("DisplayTableGoToToStep2")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToDisplayTableStep2(DisplayTableStep1ViewModel dTVM)
    {
        if (ModelState.IsValid)
        {
            ObjectPool.DisplayTableStep2ViewModel = new DisplayTableStep2ViewModel()
            {
                Table = new Table()
                {
                    Name = dTVM.TableName,
                    NumberOfColumns = GetColumnNames(dTVM.TableName).Count,
                    Columns = GetColumns(dTVM.TableName)
                },
                Rows = GetTableRows(dTVM.TableName)
            };

            return RedirectToAction("DisplayTableStep2");
        }

        return RedirectToAction("DisplayTableStep1");
    }

    [HttpPost, ActionName("DisplayTableGoToStep3")]
    [ValidateAntiForgeryToken]
    public IActionResult GoToDisplayTableStep3()
    {
        return RedirectToAction("DisplayTableStep3");
    }
    #endregion

    #region Export Table
    [HttpPost, ActionName("ExportTable")]
    [ValidateAntiForgeryToken]
    public IActionResult ExportingTable(ExportTableViewModel eVM)
    {
        SaveTable(eVM.TableName);

        string exportPath = Path.Combine(_webEnvironmentRootDirectory, "Export.csv");

        if (System.IO.File.Exists(exportPath))
        {
            return PhysicalFile(exportPath, "text/csv", Path.GetFileName(exportPath));
        }
        else
        {
            return RedirectToAction("ExportTableStep1");
        }
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

    private void AddColumnToXmlDb()
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNode headerNode = xml.DocumentElement
                                .SelectSingleNode($"//database/table[@name='{ObjectPool.AddColumnStep2ViewModel.Table.Name}']/header");
        XmlElement newColumn = xml.CreateElement("Column");
        XmlAttribute newColumnName = xml.CreateAttribute("name");

        newColumnName.Value = ObjectPool.AddColumnStep2ViewModel.Table.Columns[ObjectPool.AddColumnStep2ViewModel.Table.NumberOfColumns - 1].Name;
        newColumn.Attributes.Append(newColumnName);
        headerNode.AppendChild(newColumn);

        XmlNodeList rowNodes = xml.DocumentElement
                                  .SelectNodes($"//database/table[@name='{ObjectPool.AddColumnStep2ViewModel.Table.Name}']/Row");

        for (int i = 0; i < rowNodes.Count; i++)
        {
            XmlElement newDataPoint = xml.CreateElement("DataPoint");
            XmlAttribute newDataPointName = xml.CreateAttribute("name");
            XmlAttribute newDataPointData = xml.CreateAttribute("data");

            newDataPointName.Value = newColumnName.Value;
            newDataPointData.Value = ObjectPool.AddColumnStep2ViewModel
                                               .TableRows[i]
                                               .DataPoints[ObjectPool.AddColumnStep2ViewModel.Table.NumberOfColumns - 1]
                                               .Data;
            newDataPoint.Attributes.Append(newDataPointName);
            newDataPoint.Attributes.Append(newDataPointData);
            rowNodes[i].AppendChild(newDataPoint);
        }

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

    private void DeleteRowFromXmlDb(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? rows = xml.DocumentElement
                               .SelectNodes($"//database/table[@name='{tableName}']/Row");

        for (int i = 0; i < rows.Count; i++)
        {
            if (int.Parse(rows[i].Attributes["id"].Value) == ObjectPool.RemoveDataRowStep1ViewModel.DeleteRowId)
            {
                xml.DocumentElement
                   .SelectSingleNode($"//database/table[@name='{tableName}']")
                   .RemoveChild(rows[i]);
                break;
            }
        }

        rows = xml.DocumentElement
                  .SelectNodes($"//database/table[@name='{tableName}']/Row");

        for (int newRowID = 0; newRowID < rows.Count; newRowID++)
        {
            rows[newRowID].Attributes["id"].Value = $"{newRowID + 1}";
        }

        xml.Save(databasePath);
    }

    private void SaveTable(string tableName)
    {
        string exportPath = Path.Combine(_webEnvironmentRootDirectory, "Export.csv");

        using (StreamWriter writer = new StreamWriter(exportPath, false))
        {
            string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
            XmlDocument xml = new XmlDocument();

            xml.Load(databasePath);

            XmlNode? headerNode = xml.DocumentElement
                                     .SelectSingleNode($"//database/table[@name='{tableName}']/header");
            string headerRow = string.Empty;

            for(int i = 0; i < headerNode.ChildNodes.Count; i++)
            {
                if (i != (headerNode.ChildNodes.Count - 1))
                {
                    headerRow = string.Concat(headerRow, headerNode.ChildNodes[i].Attributes["name"].Value, ",");
                }
                else
                {
                    headerRow = string.Concat(headerRow, headerNode.ChildNodes[i].Attributes["name"].Value);
                }
            }

            writer.WriteLine(headerRow);

            XmlNodeList? rowNodes = xml.DocumentElement
                                       .SelectNodes($"//database/table[@name='{tableName}']/Row");

            foreach(XmlNode rowNode in rowNodes)
            {
                string rowData = string.Empty;

                for (int i = 0; i < rowNode.ChildNodes.Count; i++)
                {
                    if (i != (rowNode.ChildNodes.Count - 1))
                    {
                        rowData = string.Concat(rowData, rowNode.ChildNodes[i].Attributes["data"].Value, ",");
                    }
                    else
                    {
                        rowData = string.Concat(rowData, rowNode.ChildNodes[i].Attributes["data"].Value);
                    }
                }

                writer.WriteLine(rowData);
            }
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
            List<string> tableNames = new List<string>(0);

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

    private List<Row> GetTableRows(string tableName)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? rows = xml.DocumentElement
                               .SelectNodes($"//database/table[@name='{tableName}']/Row");
        List<Row> tableRows = new List<Row>(0);

        foreach (XmlNode row in rows)
        {
            tableRows.Add(new Row()
            {
                Id = int.Parse(row.Attributes["id"].Value),
                Columns = GetColumns(tableName),
                DataPoints = GetRowDataPoints(tableName, int.Parse(row.Attributes["id"].Value))
            });
        }

        return tableRows;
    }

    private List<DataPoint> GetRowDataPoints(string tableName, int rowId)
    {
        string databasePath = Path.Combine(_webEnvironmentRootDirectory, "ResultsDatabase.xml");
        XmlDocument xml = new XmlDocument();

        xml.Load(databasePath);

        XmlNodeList? documentDataPoints = xml.DocumentElement
                                             .SelectNodes($"//database/table[@name='{tableName}']/Row[@id='{rowId}']/DataPoint");
        List<DataPoint> dataPoints = new List<DataPoint>(0);

        foreach (XmlNode documentDataPoint in documentDataPoints)
        {
            dataPoints.Add(new DataPoint()
            {
                Data = documentDataPoint.Attributes["data"].Value
            });
        }

        return dataPoints;
    }
    #endregion
}

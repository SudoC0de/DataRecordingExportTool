using System.ComponentModel.DataAnnotations;

namespace DataRecordingExportTool.Models;

public class Table
{
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "(Test Validation Error) Column Name can only accpet letters and numbers!")]
    [StringLength(1, ErrorMessage = "(Test Validation Error) Column Name cannot be more than 1 character!")]
    [Required]
    public string Name { get; set; }
    public List<Column> Columns { get; set; }

    public Table()
    {
        Name = string.Empty;
        Columns = new List<Column>(0);
    }
}

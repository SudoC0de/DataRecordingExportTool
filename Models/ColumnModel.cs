using System.ComponentModel.DataAnnotations;

namespace DataRecordingExportTool.Models;

public class Column
{
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "(Test Validation Error) Column Name can only accpet letters and numbers!")]
    [StringLength(1, ErrorMessage = "(Test Validation Error) Column Name cannot be more than 1 character!")]
    [Required]
    public string ColumnHeader { get; set; }

    public Column()
    {
        ColumnHeader = string.Empty;
    }
}

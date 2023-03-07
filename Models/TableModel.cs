using System.ComponentModel.DataAnnotations;

namespace DataRecordingExportTool.Models;

public class Table
{
    [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Table Name can only be Letters and Numbers!")]
    [StringLength(2, MinimumLength = 1)]
    [Display(Name = "Data Table Name")]
    [Required]
    public string Name { get; set; }

    [Range(1,99)]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Number of columns can only contain Numbers!")]
    [Display(Name = "Number of Columns")]
    [Required]
    public int NumberOfColumns { get; set; }

    public Table()
    {
        Name = string.Empty;
    }
}

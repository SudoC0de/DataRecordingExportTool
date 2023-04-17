using System.ComponentModel.DataAnnotations;

namespace DataRecordingExportTool.Models.Objects;

public class Column
{
    [MinLength(1)]
    [RegularExpression(@"^[a-zA-Z0-9_.]*$", ErrorMessage = "Column Name can only accept letters, numbers, underscores, and periods!")]
    [StringLength(30, ErrorMessage = "Column Name cannot be more than 30 characters!")]
    [Required]
    public string Name { get; set; }

    public Column()
    {
        Name = string.Empty;
    }
}

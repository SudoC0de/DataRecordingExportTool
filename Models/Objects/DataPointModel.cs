using System.ComponentModel.DataAnnotations;

namespace DataRecordingExportTool.Models.Objects
{
    public class DataPoint
    {
        [MinLength(1)]
        [RegularExpression(@"^[a-zA-Z0-9!@#\$%\^&\*\(\)_\-\+=\{\}\[\]\\|:;""'<>,\.\?\/`~]*$", ErrorMessage = "Data can only accept letters, numbers, and special characters!")]
        [StringLength(30, ErrorMessage = "Data cannot be more than 30 characters!")]
        [Required]
        public string Data { get; set; }

        public DataPoint()
        {
            Data = string.Empty;
        }
    }
}

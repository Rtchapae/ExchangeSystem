using System.Text.Json.Serialization;

namespace ExchangeSystem.Models
{
    public class JsonRoot
    {
        [JsonPropertyName("MatItem")]
        public List<MatItem> MatItem { get; set; } = new List<MatItem>();
    }

    public class MatItem
    {
        [JsonPropertyName("GroupMatId")]
        public int GroupMatId { get; set; }

        [JsonPropertyName("MatId")]
        public int MatId { get; set; }

        [JsonPropertyName("MeasureId")]
        public int MeasureId { get; set; }

        [JsonPropertyName("NameGroupMat")]
        public string NameGroupMat { get; set; } = string.Empty;

        [JsonPropertyName("NameMat")]
        public string NameMat { get; set; } = string.Empty;

        [JsonPropertyName("NameMeasure")]
        public string NameMeasure { get; set; } = string.Empty;
    }

    public class SvsMaterial
    {
        public int Id { get; set; }
        public int GroupMatId { get; set; }
        public int MatId { get; set; }
        public int MeasureId { get; set; }
        public string NameGroupMat { get; set; } = string.Empty;
        public string NameMat { get; set; } = string.Empty;
        public string NameMeasure { get; set; } = string.Empty;
        public int? EducationDepartmentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual EducationDepartment? EducationDepartment { get; set; }
    }
}

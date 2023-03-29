using System.Text.Json;
using System.Text.Json.Serialization;

namespace AndersenTestingTask.Domain.Models;

public class FilterModel
{
    [JsonPropertyName("minprice")]
    public decimal? MinPrice { get; set; }
    [JsonPropertyName("maxprice")]
    public decimal? MaxPrice { get; set; }

    [JsonPropertyName("size")] public string? Size { get; set; } 

    [JsonPropertyName("highlight")] public string? Highlight { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
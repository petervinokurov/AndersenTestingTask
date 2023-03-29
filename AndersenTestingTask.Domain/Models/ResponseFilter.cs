namespace AndersenTestingTask.Domain.Models;

public class ResponseFilter
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string[] Sizes { get; set; }
    public string[] Words { get; set; } = new string[10];
}
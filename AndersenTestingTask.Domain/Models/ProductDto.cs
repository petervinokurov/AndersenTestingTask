namespace AndersenTestingTask.Domain.Models;

public class ProductDto
{
    public string Title { get; set; }

    public decimal Price { get; set; }

    public List<string> Sizes { get; set; }

    public string Description { get; set; }
}
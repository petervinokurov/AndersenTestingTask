namespace AndersenTestingTask.Domain.Models;

public class ContextModel
{
    public IEnumerable<Product> Products { get; set; } = new List<Product>();
}
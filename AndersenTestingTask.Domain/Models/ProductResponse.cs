namespace AndersenTestingTask.Domain.Models;

public class ProductResponse
{
    public ResponseFilter FilterObject { get; set; } = new ResponseFilter();
    public List<ProductDto> Products { get; set; } = new List<ProductDto>();
}
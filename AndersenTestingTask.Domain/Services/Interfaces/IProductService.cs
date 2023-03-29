using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductDto>> GetProducts(FilterModel filter);
}
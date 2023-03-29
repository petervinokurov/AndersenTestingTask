using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Services.Interfaces;

public interface IProductService
{
    Task<ProductResponse> GetProductsAsync(FilterModel filter);
}
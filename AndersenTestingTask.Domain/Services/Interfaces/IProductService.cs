using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Services.Interfaces;

public interface IProductService
{
    Task<ProductResponse> GetProducts(FilterModel filter);
}
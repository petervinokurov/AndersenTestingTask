using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Domain.Repositories.Interfaces;

public interface IProductCache
{
    ResponseFilter GetFilterObject(IEnumerable<Product> products);
}
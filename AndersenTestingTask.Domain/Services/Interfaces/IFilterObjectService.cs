using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Domain.Services.Interfaces;

public interface IFilterObjectService
{
    ResponseFilter GetFilterObject(IEnumerable<Product> products);
}
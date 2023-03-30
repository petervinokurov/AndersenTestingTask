using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Domain.Services.Interfaces;

public interface IFilterObjectProvider
{
    ResponseFilter GetFilterObject(IEnumerable<Product> products);
}
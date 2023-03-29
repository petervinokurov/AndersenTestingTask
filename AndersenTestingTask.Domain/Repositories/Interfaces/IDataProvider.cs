using AndersenTestingTask.Domain.Models;

namespace AndersenTestingTask.Domain.Repositories.Interfaces;

public interface IDataProvider
{
    Task<ContextModel> Products();
}
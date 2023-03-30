using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Repositories.Interfaces;
using AndersenTestingTask.Domain.Services.Interfaces;
using AndersenTestingTask.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AndersenTestingTask.Domain.Services;

public class ProductsService : IProductService
{
    private const string DataKey = "Data";
    private const string FilterObjectKey = "FilterObject";
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProductsService> _logger;
    private readonly IDataProvider _dataProvider;
    private readonly IFilterObjectService _filterObjectService;

    public ProductsService(IMemoryCache cache,
        ILogger<ProductsService> logger,
        IDataProvider dataProvider,
        IFilterObjectService filterObjectService)
    {
        _cache = cache;
        _logger = logger;
        _dataProvider = dataProvider;
        _filterObjectService = filterObjectService;
    }
    public async Task<ProductResponse> GetProductsAsync(FilterModel filter)
    {
        var response = new ProductResponse();
        var products = _cache.Get<IEnumerable<Product>>(DataKey);
        var filterObject = _cache.Get<ResponseFilter>(FilterObjectKey);
        
        // Reload the cache if something is not set in it. OR statement selected because of data consistency issue.
        if (products == null || filterObject == null)
        {
            var data = await _dataProvider.Products();
            _cache.Set(DataKey, data.Products);
            products = data.Products.ToList();

            filterObject = _filterObjectService.GetFilterObject(products);
            _cache.Set(FilterObjectKey, filterObject);
        }
        else
        {
            _logger.LogInformation($"Data found in cache.");
        }
        
        var sizes = filter.Size?.Split(',').ToList() ?? new List<string>();
        var highlights = filter.Highlight?.Split(',').ToList() ?? new List<string>();

        if (filter.MinPrice.HasValue)
        {
            products = products.Where(x => x.Price >= filter.MinPrice);
        }

        if (filter.MaxPrice.HasValue)
        {
            products = products.Where(x => x.Price <= filter.MaxPrice);
        }

        if (sizes.Any()) 
        {
            products = products.Where(x => x.Sizes.Intersect(sizes).Count() == sizes.Count);
        }

        response.Products = products.Select(x =>
            new ProductDto
            {
                Price = x.Price, 
                Title = x.Title,
                Description = x.CustomDescription(highlights), 
                Sizes = x.Sizes 
            }
        ).ToList();
        
        response.FilterObject = filterObject;
        _logger.LogInformation($"Filter applied with result {response.Products.Count} entries.");
        
        return response;
    }
}
using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Repositories.Interfaces;
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

    public ProductsService(IMemoryCache cache,
        ILogger<ProductsService> logger,
        IDataProvider dataProvider)
    {
        _cache = cache;
        _logger = logger;
        _dataProvider = dataProvider;
    }
    public async Task<ProductResponse> GetProducts(FilterModel filter)
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
            response.FilterObject = new ResponseFilter
            {
                Sizes = products.SelectMany(x => x.Sizes).Distinct().ToArray(),
                MinPrice = products.Select(x => x.Price).Min(),
                MaxPrice = products.Select(x => x.Price).Max(),
                Words = products.SelectMany(x => x.Description.Split(" "))
                    .GroupBy(x => x).Select(x => new { x.Key, Count = x.Count() })
                    .OrderByDescending(x => x.Count).Skip(5).Take(10).Select(x => x.Key).ToArray() 
            };
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
                    { Price = x.Price, Title = x.Title,
                        Description = x.CustomDescription(highlights), 
                        Sizes = x.Sizes }
            
        ).ToList();
        _logger.LogInformation($"Filter applied with result {response.Products.Count} entries.");
        return response;
    }

}
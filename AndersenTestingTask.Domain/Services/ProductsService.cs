using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace AndersenTestingTask.Domain.Services;

public class ProductsService : IProductService
{
    private const string DataUrl = "http://www.mocky.io/v2/5e307edf3200005d00858b49";
    private readonly IMemoryCache _cache;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(IMemoryCache cache, ILogger<ProductsService> logger)
    {
        _cache = cache;
        _logger = logger;
    }
    public async Task<List<ProductDto>> GetProducts(FilterModel filter)
    {
        var products = _cache.Get<IEnumerable<Product>>(nameof(DataUrl));
        if (products == null)
        {
            using var client = new RestClient();
            {
                var response = await client.ExecuteAsync<ContextModel>(new RestRequest(DataUrl));
                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    _logger.LogInformation($"Data request executed with {response.Data?.Products.Count()} entries.");
                }
                else
                {
                    _logger.LogError($"{response.StatusDescription}");
                }

                _cache.Set(nameof(DataUrl), response.Data?.Products);
                products = response.Data?.Products ?? new List<Product>();
            }
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

        if (highlights.Any())
        {
            products = products.Where(x => x.Description.Split(" ").Intersect(highlights).Any());
        }

        var result = products.Select(x =>
            new ProductDto
                    { Price = x.Price, Title = x.Title,
                        Description = x.CustomDescription(highlights), 
                        Sizes = x.Sizes }
            
        ).ToList();
        _logger.LogInformation($"Filter applied with result {result.Count} entries.");
        return result;
    }

}
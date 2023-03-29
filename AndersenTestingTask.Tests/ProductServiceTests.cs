using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace AndersenTestingTask.Tests;

public class ProductServiceTests
{
    private readonly ProductsService _service;
    
    public ProductServiceTests()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var mock = new Mock<ILogger<ProductsService>>();
        cache.Set("DataUrl", GetTestSessions());
        _service = new ProductsService(cache, mock.Object);
    }
    
    [Fact]
    public async Task EmptyFilterShouldReturnAllProducts()
    {
        var result = await _service.GetProducts(new FilterModel());
        Assert.Equal(3, result.Count);
    }
    
    [Fact]
    public async Task FilterWithMinPriceShouldReturnProductsWithPriceGreaterThanMinPriceInFilter()
    {
        var result = await _service.GetProducts(new FilterModel { MinPrice = 15 });
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.True(x.Price >= 15));
    }
    
    [Fact]
    public async Task FilterWithMaxPriceShouldReturnProductsWithPriceLessThanMinPriceInFilter()
    {
        var result = await _service.GetProducts(new FilterModel { MaxPrice = 15 });
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.True(x.Price <= 15));
    }
    
    [Fact]
    public async Task FilterWithOneSizeShouldReturnProductsWithListOfSizesWhereWillBeSizeFromFilter()
    {
        var result = await _service.GetProducts(new FilterModel { Size = "small" });
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Contains("small", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithTwoSizesShouldReturnProductsWithListOfSizesWhereWillBeSizesFromFilter()
    {
        var result = await _service.GetProducts(new FilterModel { Size = "small,large" });
        Assert.Single(result);
        Assert.All(result, x => Assert.Contains("small", x.Sizes));
        Assert.All(result, x => Assert.Contains("large", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithHighlightShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProducts(new FilterModel { Highlight = "green" });
        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Contains("<em>green</em>", x.Description));
    }
    
    [Fact]
    public async Task FilterWithHighlightSeparatedByCommaShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProducts(new FilterModel { Highlight = "green,red" });
        Assert.Equal(3, result.Count);
    }

    private List<Product> GetTestSessions()
    {
        return new List<Product>
        {
            new Product
            {
                Title = "Test Title 1", Description = "green red", Price = 10,
                Sizes = new List<string> { "small", "medium" }
            },
            new Product
            {
                Title = "Test Title 2", Description = "blue red", Price = 15,
                Sizes = new List<string> { "small", "large" }
            },
            new Product
            {
                Title = "Test Title 3", Description = "green blue", Price = 20,
                Sizes = new List<string> { "large", "medium" }
            }
            
        };
    }
}
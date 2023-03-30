using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Services;
using AndersenTestingTask.Domain.Repositories.Interfaces;
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
        var dataProviderMock = new Mock<IDataProvider>();
        dataProviderMock.Setup(x => x.Products()).ReturnsAsync(GetContextModel());
        cache.Set("DataUrl", GetTestProducts());
        _service = new ProductsService(cache, mock.Object, dataProviderMock.Object, new FilterObjectService());
    }
    
    [Fact]
    public async Task EmptyFilterShouldReturnAllProducts()
    {
        var result = await _service.GetProductsAsync(new FilterModel());
        Assert.Equal(3, result.Products.Count);
    }
    
    [Fact]
    public async Task FilterWithMinPriceShouldReturnProductsWithPriceGreaterThanMinPriceInFilter()
    {
        var result = await _service.GetProductsAsync(new FilterModel { MinPrice = 15 });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.True(x.Price >= 15));
    }
    
    [Fact]
    public async Task FilterWithMaxPriceShouldReturnProductsWithPriceLessThanMinPriceInFilter()
    {
        var result = await _service.GetProductsAsync(new FilterModel { MaxPrice = 15 });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.True(x.Price <= 15));
    }
    
    [Fact]
    public async Task FilterWithOneSizeShouldReturnProductsWithListOfSizesWhereWillBeSizeFromFilter()
    {
        var result = await _service.GetProductsAsync(new FilterModel { Size = "small" });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.Contains("small", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithTwoSizesShouldReturnProductsWithListOfSizesWhereWillBeSizesFromFilter()
    {
        var result = await _service.GetProductsAsync(new FilterModel { Size = "small,large" });
        Assert.Single(result.Products);
        Assert.All(result.Products, x => Assert.Contains("small", x.Sizes));
        Assert.All(result.Products, x => Assert.Contains("large", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithHighlightShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProductsAsync(new FilterModel { Highlight = "green" });
        Assert.Equal(3, result.Products.Count);
        Assert.Contains(result.Products, x => x.Description.Contains("<em>green</em>"));
    }
    
    [Fact]
    public async Task FilterWithHighlightSeparatedByCommaShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProductsAsync(new FilterModel { Highlight = "green,red" });
        Assert.Equal(3, result.Products.Count);
    }
    
    [Fact]
    public async Task FilterObjectShouldSetInResponse()
    {
        var result = await _service.GetProductsAsync(new FilterModel { Highlight = "green,red" });
        Assert.Equal(20, result.FilterObject.MaxPrice);
        Assert.Equal(10, result.FilterObject.MinPrice);
        Assert.Contains("small", result.FilterObject.Sizes);
        Assert.Contains("medium", result.FilterObject.Sizes);
        Assert.Contains("large", result.FilterObject.Sizes);
        Assert.Contains("green", result.FilterObject.Words);
        Assert.Contains("red", result.FilterObject.Words);
        Assert.Contains("blue", result.FilterObject.Words);
    }

    private ContextModel GetContextModel()
    {
        return new ContextModel { Products = GetTestProducts() };
    }
    private List<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Title = "Test Title 1", Description = "a b c d e green the red", Price = 10,
                Sizes = new List<string> { "small", "medium" }
            },
            new Product
            {
                Title = "Test Title 2", Description = "a b c d e blue the red", Price = 15,
                Sizes = new List<string> { "small", "large" }
            },
            new Product
            {
                Title = "Test Title 3", Description = "a b c d e green the blue", Price = 20,
                Sizes = new List<string> { "large", "medium" }
            }
            
        };
    }
}
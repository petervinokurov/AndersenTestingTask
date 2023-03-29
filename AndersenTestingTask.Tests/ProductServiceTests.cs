using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Repositories.Interfaces;
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
        var dataProviderMock = new Mock<IDataProvider>();
        dataProviderMock.Setup(x => x.Products()).ReturnsAsync(GetContextModel());
        cache.Set("DataUrl", GetTestProducts());
        _service = new ProductsService(cache, mock.Object, dataProviderMock.Object);
    }
    
    [Fact]
    public async Task EmptyFilterShouldReturnAllProducts()
    {
        var result = await _service.GetProducts(new FilterModel());
        Assert.Equal(3, result.Products.Count);
    }
    
    [Fact]
    public async Task FilterWithMinPriceShouldReturnProductsWithPriceGreaterThanMinPriceInFilter()
    {
        var result = await _service.GetProducts(new FilterModel { MinPrice = 15 });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.True(x.Price >= 15));
    }
    
    [Fact]
    public async Task FilterWithMaxPriceShouldReturnProductsWithPriceLessThanMinPriceInFilter()
    {
        var result = await _service.GetProducts(new FilterModel { MaxPrice = 15 });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.True(x.Price <= 15));
    }
    
    [Fact]
    public async Task FilterWithOneSizeShouldReturnProductsWithListOfSizesWhereWillBeSizeFromFilter()
    {
        var result = await _service.GetProducts(new FilterModel { Size = "small" });
        Assert.Equal(2, result.Products.Count);
        Assert.All(result.Products, x => Assert.Contains("small", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithTwoSizesShouldReturnProductsWithListOfSizesWhereWillBeSizesFromFilter()
    {
        var result = await _service.GetProducts(new FilterModel { Size = "small,large" });
        Assert.Single(result.Products);
        Assert.All(result.Products, x => Assert.Contains("small", x.Sizes));
        Assert.All(result.Products, x => Assert.Contains("large", x.Sizes));
    }
    
    [Fact]
    public async Task FilterWithHighlightShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProducts(new FilterModel { Highlight = "green" });
        Assert.Equal(3, result.Products.Count);
        Assert.Contains(result.Products, x => x.Description.Contains("<em>green</em>"));
    }
    
    [Fact]
    public async Task FilterWithHighlightSeparatedByCommaShouldReturnProductsWithMarkedWordsInDescription()
    {
        var result = await _service.GetProducts(new FilterModel { Highlight = "green,red" });
        Assert.Equal(3, result.Products.Count);
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
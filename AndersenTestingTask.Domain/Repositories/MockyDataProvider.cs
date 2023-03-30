using System.Text.Json;
using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Domain.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace AndersenTestingTask.Domain.Repositories;

public class MockyDataProvider : IDataProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<MockyDataProvider> _logger;
    
    public MockyDataProvider(IConfiguration configuration, 
        ILogger<MockyDataProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task<ContextModel> Products()
    {
        var model = new ContextModel();
        
        using var client = new RestClient();
        {
            var urlResponse = await client.ExecuteAsync<ContextModel>(new RestRequest(_configuration["DataUrl"]));
            model.Products = urlResponse.Data?.Products ?? new List<Product>();
            _logger.LogInformation(JsonSerializer.Serialize(urlResponse));
        }
        
        return model;
    }
}
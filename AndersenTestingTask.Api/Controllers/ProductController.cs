using AndersenTestingTask.Domain.Models;
using AndersenTestingTask.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AndersenTestingTask.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _context;
    private readonly ILogger<ProductController> _logger;
    public ProductController(IProductService context, ILogger<ProductController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public Task<List<ProductDto>> Filter([FromQuery]FilterModel filter)
    {
        _logger.LogInformation($"Filter applied: {filter}");
        return _context.GetProducts(filter);
    }
}
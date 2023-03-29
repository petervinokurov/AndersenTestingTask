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

    // The method naming doesn't align with the REST convention.
    [HttpGet]
    public Task<ProductResponse> Filter([FromQuery]FilterModel filter)
    {
        _logger.LogInformation($"Filter applied: {filter}");
        return _context.GetProducts(filter);
    }
}
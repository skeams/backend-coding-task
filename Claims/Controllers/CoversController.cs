using Claims.Models;
using Claims.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IValidator<Cover> _validator;
    private readonly CoversService _service;

    public CoversController(ILogger<CoversController> logger, CoversService service, IValidator<Cover> validator)
    {
        _logger = logger;
        _service = service;
        _validator = validator;
    }
    
    [HttpPost("compute-premium")]
    public ActionResult ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
    {
        var premium = _service.ComputePremium(startDate, endDate, coverType);
        return Ok(premium);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var covers = await _service.GetCovers();
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        try
        {
            var response = await _service.GetCover(id);
            return Ok(response);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        var validationResult = await _validator.ValidateAsync(cover);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToString());
        }

        var computedCover = await _service.CreateCover(cover);
        return Ok(computedCover);
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _service.DeleteCover(id);
    }
}
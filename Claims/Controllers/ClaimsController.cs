using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        
        private readonly ILogger<ClaimsController> _logger;
        private readonly ClaimsService _service;

        public ClaimsController(ILogger<ClaimsController> logger, ClaimsService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _service.GetClaims();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
             * TODO: Om tid, legg til validering av dato opp mot cover-id:
             *
             * Created date must be within the period of the related Cover
             */

            await _service.CreateClaim(claim);
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(string id)
        {
            return _service.DeleteClaim(id);
        }

        [HttpGet("{id}")]
        public Task<Claim> GetAsync(string id)
        {
            return _service.GetClaim(id);
        }
    }
}
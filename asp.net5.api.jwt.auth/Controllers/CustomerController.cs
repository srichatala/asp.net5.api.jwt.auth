using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Controllers
{
    [Route("api/customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IJwtAuthService _jwtAuthService;
        public CustomerController(ICustomerService customerService, IJwtAuthService jwtAuthService)
        {
            _customerService = customerService;
            _jwtAuthService = jwtAuthService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var isValid = await _customerService.IsValidateCustomer(request);
            if (!isValid)
            {
                return Unauthorized();
            }
            var jwtResult = _jwtAuthService.GenerateTokens(request.UserName);
            return Ok(new JwtResponse
            {
                UserName = request.UserName,
                AccessToken = jwtResult.AccessToken,
            });
        }

        [AllowAnonymous]
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] Customer request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            bool registered = await _customerService.Regstration(request);
            if (!registered)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Somethng went wrong");

            }
            var jwtResult = _jwtAuthService.GenerateTokens(request.UserName);
            return Ok(new JwtResponse
            {
                UserName = request.UserName,
                AccessToken = jwtResult.AccessToken,
            });
        }

        [HttpGet("customer")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userName = User.Identity?.Name;
            var customer = await _customerService.GetCustomer(userName);
            if(customer != null)
            {
                return Ok(customer);
            }
            return NoContent();
        }
    }
}

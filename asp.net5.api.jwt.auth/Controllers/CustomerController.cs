using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asp.net5.api.jwt.auth.Controllers
{
    [Route("api/[controller]")]
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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_customerService.IsValidateCustomer(request))
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

        [HttpGet("customer")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userName = User.Identity?.Name;
            var customer = _customerService.GetCustomer(userName);
            return Ok(customer);
        }
    }
}

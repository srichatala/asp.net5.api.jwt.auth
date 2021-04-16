using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
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
            return Ok(new LoginResult
            {
                UserName = jwtResult.RefreshToken.UserName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
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
            var jwtResult = _jwtAuthService.GenerateTokens(request.Email);
            return Ok(new LoginResult { 
                UserName = jwtResult.RefreshToken.UserName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Unauthorized();
            }

            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
            var jwtResult = _jwtAuthService.Refresh(request.RefreshToken, accessToken, DateTime.Now);
            return Ok(new LoginResult
            {
                UserName = userName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }

        [HttpGet("customer")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userName = User?.FindFirst(x => x.Type.Equals("Name"))?.Value;
            var customer = await _customerService.GetCustomer(userName);
            if(customer != null)
            {
                return Ok(customer);
            }
            return NoContent();
        }
    }
}

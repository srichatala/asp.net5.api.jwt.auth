using asp.net5.api.jwt.auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        [HttpGet("products")]
        public IActionResult GetAction()
        {
            var list = new List<ProductDto>()
            {
                new ProductDto
                {
                    Id = 1,
                    Name = "Asus ROG",
                    Price = 1500
                },
                 new ProductDto
                {
                    Id = 2,
                    Name = "iPhone ",
                    Price = 1100
                },
            };

            return Ok(list);
        }
    }
}

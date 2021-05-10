using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Controllers
{
    [Route("api/post")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet("posts")]
        public async Task<IActionResult> GetAction()
        {
            var list = await _postService.GetPosts();
            return Ok(list);
        }

        [HttpPost("createpost")]
        public async Task<IActionResult> CreatePost(Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var list = await _postService.CreatePost(post);
            return Ok(list);
        }
    }
}

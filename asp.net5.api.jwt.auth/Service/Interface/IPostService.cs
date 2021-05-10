using asp.net5.api.jwt.auth.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service.Interface
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetPosts();
        Task<IEnumerable<PostDto>> CreatePost(Post post);
    }
}

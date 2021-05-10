using asp.net5.api.jwt.auth.Model;
using asp.net5.api.jwt.auth.Service.Interface;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Service
{
    public class PostService : IPostService
    {
        private readonly AppSettings _appSettings;
        public PostService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public async Task<IEnumerable<PostDto>> GetPosts()
        {
            var posts = new List<PostDto>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
                {
                    var query = "SELECT * FROM [dbo].[Post] ORDER BY UpdateDate DESC";
                    posts = (await sqlConnection.QueryAsync<PostDto>(query, sqlConnection)).ToList();
                }
            }
            catch (Exception)
            {

            }
            return posts;
        }

        public async Task<IEnumerable<PostDto>> CreatePost(Post post)
        {
            var list = new List<PostDto>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_appSettings.ConnectionString))
                {
                    var query = "INSERT INTO [dbo].[Post] (Name,Description,UserId,CreateDate,UpdateDate)Values(@Name,@Description,@UserId,@CreateDate,@UpdateDate)";
                    var affectedRows = await sqlConnection.ExecuteAsync(query, new { Name = post.Name, Description = post.Description, UserId = post.UserId, CreateDate = DateTime.UtcNow, UpdateDate = DateTime.UtcNow });
                    if (affectedRows > 0)
                    {
                        list = (await GetPosts()).ToList();
                    }
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Something went wrong");
            }
            return list;
        }
    }
}

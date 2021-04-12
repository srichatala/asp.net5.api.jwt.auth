using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Model
{
    public class JwtResponse
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}

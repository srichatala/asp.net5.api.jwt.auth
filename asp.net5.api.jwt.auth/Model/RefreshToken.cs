﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Model
{
    public class RefreshToken
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }    // can be used for usage tracking
                                                // can optionally include other metadata, such as user agent, ip address, device name, and so on
        [JsonPropertyName("tokenString")]
        public string TokenString { get; set; }

        [JsonPropertyName("expireAt")]
        public DateTime ExpireAt { get; set; }
    }
}

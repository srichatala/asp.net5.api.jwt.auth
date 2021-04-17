using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace asp.net5.api.jwt.auth.Model
{
    public class ProductDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("productName")]
        public string ProductName { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}

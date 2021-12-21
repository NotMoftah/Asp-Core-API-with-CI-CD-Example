using System;
using System.ComponentModel.DataAnnotations;

namespace SlsApi.Models
{
    public class LoginResponse
    {
        public string? ServerId { get; set; }
        public string? JWTToken { get; set; }
    }
}

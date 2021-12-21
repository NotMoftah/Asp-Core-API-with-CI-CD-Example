using System;
using System.ComponentModel.DataAnnotations;

namespace SlsApi.Models
{
    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }        
    }
}

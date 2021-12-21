using System;
using System.ComponentModel.DataAnnotations;

namespace SlsApi.Models
{
    public class LoginModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string? Password { get; set; }
    }
}

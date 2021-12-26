using System;
using System.ComponentModel.DataAnnotations;

namespace SlsApi.Models
{
    public class UsernamePasswordModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email Is Required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}

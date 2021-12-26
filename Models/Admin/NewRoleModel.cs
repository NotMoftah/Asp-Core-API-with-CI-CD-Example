using System;
using System.ComponentModel.DataAnnotations;

namespace SlsApi.Models
{
    public class NewRoleModel
    {
        [Required(ErrorMessage = "Role Name Is Required")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }
    }
}

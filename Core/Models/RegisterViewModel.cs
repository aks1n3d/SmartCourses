using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models
{
    public class RegisterViewModel
    {
        [Required]
        [MaxLength(20, ErrorMessage = "Max 20 characters!")]
        [MinLength(3, ErrorMessage = "Min 4 characters!")]
        public string Login { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Min 7 characters")]
        [Required]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Compare("Password", ErrorMessage = "Password is not confirmed")]
        public string PasswordConfirm { get; set; }
    }
}

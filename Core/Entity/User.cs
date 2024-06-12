using Core.Entity.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Core.Entity
{
    public class User : BaseEntity
    {
        [Required(ErrorMessage = "Login must have up to 30 character"), MaxLength(30)]
        public string Login { get; set; }
        [Required(ErrorMessage = "Password must have at least 6 character, up to 30")]
        public string Password { get; set; }
        public Profile Profile { get; set; }
    }
}

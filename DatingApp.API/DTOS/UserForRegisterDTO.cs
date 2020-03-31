using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOS
{
    public class UserForRegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8,MinimumLength = 4,ErrorMessage = "You must specify password between 4 and 8 chars")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
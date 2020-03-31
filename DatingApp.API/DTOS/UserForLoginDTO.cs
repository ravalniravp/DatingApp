using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOS
{
    public class UserForLoginDTO
    {
        
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
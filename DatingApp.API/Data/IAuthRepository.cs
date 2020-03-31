using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user,string password);
         Task<User> LogIN(string userName,string password);
         Task<bool> UserExists(string username);
    }
}
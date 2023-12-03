using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        int Register(User user);
        User? SignIn(string username, string password);
        User? GetSingleUser(int id);
        User? GetSingleUserByEmail(string email);
        User? GetSingleUserByUsername(string username);
        List<User> GetUsers(string? searchValue);
        int ChangePassword(int userId, string newPassword);
        int UpdateUser(User u);
    }
}

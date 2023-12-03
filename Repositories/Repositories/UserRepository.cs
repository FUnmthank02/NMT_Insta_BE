using BusinessObjects.Models;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        public int ChangePassword(int userId, string newPassword)
        {
            try
            {
                return UserDAO.ChangePassword(userId, newPassword);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int Register(User user)
        {
            try
            {
                return UserDAO.AddNewUser(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public User? SignIn(string username, string password)
        {
            try
            {
                var user = UserDAO.GetUserByUsernamePassword(username, password);
                if (user != null && user.Status == true) // account is existed and activated
                {
                    return user;
                }
                return null;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public User? GetSingleUser(int id)
        {
            try
            {
                var user = UserDAO.GetUserById(id);
                if (user != null) // account is existed and activated
                {
                    return user;
                }
                return null;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int UpdateUser(User u)
        {
            return UserDAO.UpdateUser(u);
        }

        public List<User> GetUsers(string? searchValue)
        {
            return UserDAO.GetUsers(searchValue);
        }

        public User? GetSingleUserByEmail(string email)
        {
            return UserDAO.GetUserByEmail(email);
        }
        public User? GetSingleUserByUsername(string username)
        {
            return UserDAO.GetUserByUsername(username);
        }
    }
}

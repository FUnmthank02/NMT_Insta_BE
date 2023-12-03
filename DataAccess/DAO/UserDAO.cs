using AutoMapper.Execution;
using BusinessObjects.Helper;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class UserDAO
    {
        public static List<User> GetUsers(string? searchValue)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    IQueryable<User> query = context.Users;

                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        query = query.Where(p => p.Name.ToLower().Contains(searchValue.ToLower()) || p.Username.ToLower().Contains(searchValue.ToLower()));
                    }
                    return query.ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static User GetUserByUsernamePassword(string username, string password)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    User user = context.Users.Where(x => x.Username == username)
                    .AsEnumerable()
                    .SingleOrDefault(x => PasswordHelper.VerifyPassword(password, x.Password));
                    return user;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static User GetUserById(int userId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.UserId == userId);
                    if (user == null)
                    {
                        return null;
                    }

                    return user;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
        
        public static User GetUserByUsername(string username)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.Username.Equals(username));
                    if (user == null)
                    {
                        return null;
                    }

                    return user;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
        public static User GetUserByEmail(string email)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.Email.Equals(email));
                    if (user == null)
                    {
                        return null;
                    }

                    return user;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        public static int AddNewUser(User u)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Users.Add(u);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int ChangePassword(int userId, string newPassword)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var user = context.Users.FirstOrDefault(o => o.UserId == userId);
                    if (user != null)
                    {
                        user.Password = newPassword;
                        return context.SaveChanges();
                    }
                    return 0;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int UpdateUser(User u)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Entry<User>(u).State =
                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

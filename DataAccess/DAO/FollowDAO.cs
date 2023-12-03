using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class FollowDAO
    {

        public static int CreateFollow(Follower p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Followers.Add(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static int DeleteFollow(Follower p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Followers.Remove(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static Follower GetSingleFollowing(int userId, int followingId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Followers.Include(c => c.FollowerNavigation).Include(c => c.Following)
                        .FirstOrDefault(c => c.FollowerId == userId && c.FollowingId == followingId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<Follower> GetAllFollowerByUserId(int userId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Followers.Include(c => c.FollowerNavigation).Include(c => c.Following).Where(c => c.FollowingId == userId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static List<Follower> GetAllFollowingByUserId(int userId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Followers.Include(c => c.FollowerNavigation).Include(c => c.Following).Where(c => c.FollowerId == userId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

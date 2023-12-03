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
    public class FollowRepository : IFollowRepository
    {
        public int CreateFollow(Follower p)
        {
            try
            {
                return FollowDAO.CreateFollow(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public int DeleteFollow(Follower p)
        {
            try
            {
                return FollowDAO.DeleteFollow(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Follower> GetAllFollowerByUserId(int userId)
        {
            try
            {
                return FollowDAO.GetAllFollowerByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Follower> GetAllFollowingByUserId(int userId)
        {
            try
            {
                return FollowDAO.GetAllFollowingByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Follower GetSingleFollowing(int userId, int followingId)
        {
            try
            {
                return FollowDAO.GetSingleFollowing(userId, followingId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

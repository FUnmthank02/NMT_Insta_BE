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
    public class ReactionRepository : IReactionRepository
    {
        public int CreateReaction(Reaction p)
        {
            try
            {
                return ReactionDAO.CreateReaction(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public int DeleteReaction(Reaction p)
        {
            try
            {
                return ReactionDAO.DeleteReaction(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Reaction GetReactionByPostIdUserId(int postId, int userId)
        {
            try
            {
                return ReactionDAO.GetReactionByPostIdUserId(postId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

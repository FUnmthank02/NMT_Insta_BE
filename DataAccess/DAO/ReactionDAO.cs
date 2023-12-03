using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ReactionDAO
    {

        public static int CreateReaction(Reaction p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Reactions.Add(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int DeleteReaction(Reaction p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Reactions.Remove(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        
        public static Reaction GetReactionByPostIdUserId(int postId, int userId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var reaction = context.Reactions.Include(c => c.Post).Include(c => c.User).FirstOrDefault(c => c.PostId == postId && c.UserId == userId);
                    return reaction;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}

using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CommentDAO
    {

        public static int CreateComment(Comment p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Comments.Add(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static int DeleteComment(Comment p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Comments.Remove(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static Comment GetCommentById(int commentId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Comments.Include(c => c.Post).Include(c => c.User).FirstOrDefault(c => c.CommentId == commentId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int UpdateComment(Comment p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Entry<Comment>(p).State =
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

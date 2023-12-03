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
    public class CommentRepository : ICommentRepository
    {
        public int CreateComment(Comment p)
        {
            try
            {
                return CommentDAO.CreateComment(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public int DeleteComment(Comment p)
        {
            try
            {
                return CommentDAO.DeleteComment(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int UpdateComment(Comment p)
        {
            try
            {
                return CommentDAO.UpdateComment(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Comment GetCommentById(int commentId)
        {
            try
            {
                return CommentDAO.GetCommentById(commentId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

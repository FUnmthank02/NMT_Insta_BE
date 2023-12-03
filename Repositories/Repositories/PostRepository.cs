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
    public class PostRepository : IPostRepository
    {
        public int CreatePost(Post p)
        {
            try
            {
                return PostDAO.CreatePost(p);
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int DeletePost(Post p)
        {
            try
            {
                return PostDAO.DeletePost(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Post GetMostRecentPost()
        {
            try
            {
                return PostDAO.GetMostRecentPost();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Post? GetPostById(int postId)
        {
            try
            {
                return PostDAO.GetPostById(postId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Post> GetPostsByUserId(int userId)
        {
            try
            {
                return PostDAO.GetPostsByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Post> GetPostsFollowing(int userId)
        {
            try
            {
                return PostDAO.GetPostsFollowing(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int UpdatePost(Post p)
        {
            try
            {
                return PostDAO.UpdatePost(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

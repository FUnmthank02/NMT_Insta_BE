using AutoMapper.Execution;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class PostDAO
    {
        public static Post GetPostById(int postId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Posts.Include(p => p.User)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                        .ThenInclude(r => r.User)
                    .Include(p => p.Media)
                    .FirstOrDefault(p => p.PostId == postId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static Post GetMostRecentPost()
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Posts.Include(p => p.User).OrderByDescending(o => o.PostId).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static List<Post> GetPostsByUserId(int userId) // to get my all posts
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Posts.Include(p => p.User)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                        .ThenInclude(r => r.User)
                    .Include(p => p.Media).Where(p => p.UserId == userId).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<Post> GetPostsFollowing(int userId) // to get all posts of people that i am following
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var test = context.Posts.Include(x => x.User)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                    .Include(p => p.Reactions)
                        .ThenInclude(r => r.User)
                    .Include(p => p.Media)
                    .Where(p => p.User.FollowerFollowings.Any(f => f.FollowerId == userId))
                    .ToList();
                    return test;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int CreatePost(Post p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Posts.Add(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int UpdatePost(Post p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Entry<Post>(p).State =
                    Microsoft.EntityFrameworkCore.EntityState.Modified;
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int DeletePost(Post p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Posts.Remove(p);
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

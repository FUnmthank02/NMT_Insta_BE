using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IPostRepository
    {
        int CreatePost(Post p);
        Post? GetPostById(int postId);
        Post GetMostRecentPost();
        List<Post> GetPostsByUserId(int userId);
        List<Post> GetPostsFollowing(int userId);
        int UpdatePost(Post p);
        int DeletePost(Post p);
    }
}

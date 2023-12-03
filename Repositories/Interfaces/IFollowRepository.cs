using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IFollowRepository
    {
        int CreateFollow(Follower p);
        int DeleteFollow(Follower p);
        List<Follower> GetAllFollowerByUserId(int userId);
        List<Follower> GetAllFollowingByUserId(int userId);
        Follower GetSingleFollowing(int userId, int followingId);
    }
}

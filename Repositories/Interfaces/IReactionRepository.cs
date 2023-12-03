using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IReactionRepository
    {
        int CreateReaction(Reaction p);

        Reaction GetReactionByPostIdUserId(int postId, int userId);
        int DeleteReaction(Reaction p);

    }
}

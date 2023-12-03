using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICommentRepository
    {
        int CreateComment(Comment p);

        int DeleteComment(Comment p);
        int UpdateComment(Comment p);
        Comment GetCommentById(int commentId);

    }
}

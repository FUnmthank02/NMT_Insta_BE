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
    public class MediaRepository : IMediaRepository
    {
        public int CreateMedia(Medium p)
        {
            try
            {
                return MediaDAO.CreateMedia(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

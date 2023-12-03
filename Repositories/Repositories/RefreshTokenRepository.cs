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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {

        public int AddRefreshToken(RefreshToken rt)
        {
            try
            {
                return RefreshTokenDAO.AddNewRefreshToken(rt);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public RefreshToken? GetSingleRefreshToken(string rfToken)
        {
            try
            {
                var user = RefreshTokenDAO.GetRefreshTokenByRfToken(rfToken);
                if (user != null)
                {
                    return user;
                }
                return null;
            } catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public int UpdateRefreshToken(RefreshToken rt)
        {
            return RefreshTokenDAO.UpdateRefreshToken(rt);
        }
    }
}

using AutoMapper.Execution;
using BusinessObjects.Helper;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class RefreshTokenDAO
    {
        
        public static RefreshToken GetRefreshTokenByRfToken(string rfToken)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var rfTokenFound = context.RefreshTokens.FirstOrDefault(x => x.Token == rfToken);
                    if (rfTokenFound == null)
                    {
                        return null;
                    }

                    return rfTokenFound;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
       
        public static int AddNewRefreshToken(RefreshToken u)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.RefreshTokens.Add(u);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int UpdateRefreshToken(RefreshToken u)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Entry<RefreshToken>(u).State =
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

using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class MediaDAO
    {

        public static int CreateMedia(Medium p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Media.Add(p);
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

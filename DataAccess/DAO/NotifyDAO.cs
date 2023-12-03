using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class NotifyDAO
    {

        public static int UpdateAllNotification(int userId) // Mark all notifications as read
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var notificationsToUpdate = context.Notifications.Where(n => n.ToUserId == userId).ToList();
                    foreach (var notification in notificationsToUpdate)
                    {
                        notification.IsRead = true;
                    }
                    context.Notifications.UpdateRange(notificationsToUpdate);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static int CreateNotification(Notification p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Notifications.Add(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static int DeleteNotification(Notification p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Notifications.Remove(p);
                    return context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        public static Notification GetSingleNotification(int notifyId)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    var notification = context.Notifications.Include(c => c.Post).Include(c => c.ToUser).Include(c => c.FromUser).FirstOrDefault(c => c.NotifyId == notifyId);
                    return notification;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static List<Notification> GetNotificationsByUserId(int id)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    return context.Notifications
                        .Include(c => c.Post)
                            .ThenInclude(x => x.Media)
                        .Include(c => c.Post)
                            .ThenInclude(x => x.Comments)
                        .Include(c => c.Post)
                            .ThenInclude(x => x.Reactions)
                        .Include(c => c.ToUser).Include(c => c.FromUser)
                        .Where(c => c.ToUserId == id).OrderByDescending(y => y.CreatedAt).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static int UpdateNotification(Notification p)
        {
            try
            {
                using (var context = new Prn231PrjContext())
                {
                    context.Entry<Notification>(p).State =
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

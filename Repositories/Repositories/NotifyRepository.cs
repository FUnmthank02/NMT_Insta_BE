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
    public class NotifyRepository : INotifyRepository
    {
        public int CreateNotification(Notification p)
        {
            try
            {
                return NotifyDAO.CreateNotification(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int DeleteNotification(Notification p)
        {
            try
            {
                return NotifyDAO.DeleteNotification(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Notification GetSingleNotification(int notifyId)
        {
            try
            {
                return NotifyDAO.GetSingleNotification(notifyId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public int UpdateNotification(Notification p)
        {
            try
            {
                return NotifyDAO.UpdateNotification(p);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public int UpdateAllNotification(int userId)
        {
            try
            {
                return NotifyDAO.UpdateAllNotification(userId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Notification> GetNotificationsByUserId(int id)
        {
            try
            {
                return NotifyDAO.GetNotificationsByUserId(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface INotifyRepository
    {
        int CreateNotification(Notification p);

        int DeleteNotification(Notification p);
        int UpdateNotification(Notification p);
        Notification GetSingleNotification(int notifyId);
        List<Notification> GetNotificationsByUserId(int id);
        int UpdateAllNotification(int userId);

    }
}

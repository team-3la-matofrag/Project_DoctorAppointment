using Project.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.DAL.Interfaces
{
   
        public interface INotificationRepository
        {
     
            Task<Notification> CreateAsync(Notification notification);

            Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);

            Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);

            Task MarkAsReadAsync(int notificationId);

        
    }
}





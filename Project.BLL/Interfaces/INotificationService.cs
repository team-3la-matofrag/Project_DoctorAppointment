using Project.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.BLL.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(NotificationDto dto);
        Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId);
        Task MarkAsReadAsync(int notificationId);

    }
}

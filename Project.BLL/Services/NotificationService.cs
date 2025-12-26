using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.DAL.Interfaces;
using Project.DAL.Models;

namespace Project.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<NotificationDto> CreateAsync(NotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                AppointmentId = dto.AppointmentId,
                Message = dto.Message ?? throw new ArgumentException("Message is required."),
                Channel = string.IsNullOrWhiteSpace(dto.Channel) ? "System" : dto.Channel.Trim(),
                ScheduledAt = dto.ScheduledAt,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(notification);
            return MapToDto(created);
        }

        public async Task<IEnumerable<NotificationDto>> GetByUserIdAsync(int userId)
        {

            var notifications = await _repo.GetByUserIdAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadByUserIdAsync(int userId)
        {
            var notifications = await _repo.GetUnreadByUserIdAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _repo.MarkAsReadAsync(notificationId);
        }
        private static NotificationDto MapToDto(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                AppointmentId = entity.AppointmentId,
                Message = entity.Message,
                Channel = entity.Channel,
                ScheduledAt = entity.ScheduledAt,
                SentAt = entity.SentAt,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                IsRead = entity.IsRead
            };
        }
    }
}

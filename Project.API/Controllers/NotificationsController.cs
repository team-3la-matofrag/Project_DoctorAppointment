using Microsoft.AspNetCore.Mvc;
using Project.BLL.DTOs;
using Project.BLL.Interfaces;
using Project.BLL.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpPost]
        public async Task<ActionResult<NotificationDto>> Create([FromBody] NotificationDto dto)
        {
            var created = await _notificationService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserId }, created);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<NotificationDto>>> GetByUserId(int userId)
        {
            var notifications = await _notificationService.GetByUserIdAsync(userId);
            return Ok(notifications);
        }
        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<List<NotificationDto>>> GetUnreadByUserId(int userId)
        {
            var notifications = await _notificationService.GetUnreadByUserIdAsync(userId);
            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent(); 
        }
    }
}

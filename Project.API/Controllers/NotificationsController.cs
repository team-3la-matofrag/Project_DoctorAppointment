using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;

namespace Project.Controllers;

[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationsController(AppDbContext context)
    {
        _context = context;
    }

    // GET /notifications
    [HttpGet("/notifications")]
    public async Task<IActionResult> GetNotifications([FromQuery] int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(notifications);
    }

    // POST /notifications/markasread/{id}
    [HttpPost("/notifications/markasread/{id:int}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
        {
            return NotFound();
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(notification);
    }

    // GET /notifications/unread-count
    [HttpGet("/notifications/unread-count")]
    public async Task<IActionResult> UnreadCount([FromQuery] int userId)
    {
        var count = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .CountAsync();

        return Ok(new { count });
    }
}



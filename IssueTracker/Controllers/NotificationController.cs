using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ITUser> _userManager;
        private readonly INotificationService _notificationService;

        public NotificationController(
            ApplicationDbContext context,
            UserManager<ITUser> userManager,
            INotificationService notificationService
            )
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // GET: Notification
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Notifications.Include(n => n.Recipent).Include(n => n.Sender).Include(n => n.Ticket);
            return View(await applicationDbContext.ToListAsync());
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ViewResult> MyNotifications()
        {
            ITUser? appUser = await _userManager.GetUserAsync(User);

            List<Notification> notifications = appUser is not null ? await _notificationService.GetAllNotificationsForUserAsync(appUser) : new List<Notification>();

            return View(notifications);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<RedirectToActionResult> MarkAsRead(int id)
        {
            Notification? notification = await _notificationService.GetByIdAsync(id);

            if (notification is not null)
            {
                if (notification.RecipentId == (await _userManager.GetUserAsync(User)).Id)
                    await _notificationService.MarkAsRead(notification);
                else
                    return RedirectToAction(nameof(NotAuthorized));
            }

            return RedirectToAction(nameof(MyNotifications));
        }

        [HttpGet]
        public ViewResult NotAuthorized()
        {
            return View();
        }

        // GET: Notification/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Recipent)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notification/Create
        public IActionResult Create()
        {
            ViewData["RecipentId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description");
            return View();
        }

        // POST: Notification/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Message,Created,Viewed,TicketId,RecipentId,SenderId")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipentId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipentId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // GET: Notification/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["RecipentId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipentId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // POST: Notification/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Message,Created,Viewed,TicketId,RecipentId,SenderId")] Notification notification)
        {
            if (id != notification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipentId"] = new SelectList(_context.Users, "Id", "Id", notification.RecipentId);
            ViewData["SenderId"] = new SelectList(_context.Users, "Id", "Id", notification.SenderId);
            ViewData["TicketId"] = new SelectList(_context.Tickets, "Id", "Description", notification.TicketId);
            return View(notification);
        }

        // GET: Notification/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Recipent)
                .Include(n => n.Sender)
                .Include(n => n.Ticket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Notifications'  is null.");
            }
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
          return (_context.Notifications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

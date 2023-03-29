﻿using IssueTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Data;

// Telling the application to use ITUser as the class as the type for IdentityDbContext
public class ApplicationDbContext : IdentityDbContext<ITUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    // Setting up DB Sets. We can access Companies through dbContext dot notation
    // We can access these through dbContext dot notation
    public DbSet<Company> Companies { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectPriority> ProjectPriorities { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketAttachment> TicketAttachments { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketHistory> TicketHistories { get; set; }
    public DbSet<TicketPriority> TicketPriorities { get; set; }
    public DbSet<TicketStatus> TicketStatuses { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }

}
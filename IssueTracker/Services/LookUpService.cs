using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class LookUpService : ILookUpService
{
    #region Properties
    private readonly ApplicationDbContext _context;
    #endregion

    #region Constructor
    public LookUpService(ApplicationDbContext context)
    {
        _context = context;
    }
    #endregion

    #region  Get Ticket Priorities
    public async Task<List<TicketPriority>> GetTicketPrioritiesAsync()
    {
        try
        {
            return await _context.TicketPriorities.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting ticket priorities. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Ticket Statuses
    public async Task<List<TicketStatus>> GetTicketStatusesAsync()
    {
        try
        {
            return await _context.TicketStatuses.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting ticket statuses. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Ticket Types
    public async Task<List<TicketType>> GetTicketTypesAsync()
    {
        try
        {
            return await _context.TicketTypes.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting ticket types. --->  {e.Message}");
            throw;
        }
    }
    #endregion
    
    #region Get Project Prioritie
    public async Task<List<ProjectPriority>> GetProjectPrioritiesAsync()
    {
        try
        {
            return await _context.ProjectPriorities.ToListAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting project priorities. --->  {e.Message}");
            throw;
        }
    }
    #endregion
}
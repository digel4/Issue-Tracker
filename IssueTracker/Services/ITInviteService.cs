using IssueTracker.Data;
using IssueTracker.Models;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services;

public class ITInviteService : IITInviteService
{
    private readonly ApplicationDbContext _context;
    
    public ITInviteService(ApplicationDbContext context)
    {
        _context = context;
    }
    //            Console.WriteLine($"****ERROR**** - Error sending email. --->  {e.Message}");
    public async Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId)
    {
        Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

        if (invite == null)
        {
            return false;
        }

        try
        {
            invite.IsValid = false;
            invite.InviteeId = userId;
            await _context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error accepting invite. --->  {e.Message}");
            throw;
        }
    }

    public async Task AddNewInviteAsync(Invite invite)
    {
        try
        {
            await _context.Invites.AddAsync(invite);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error adding new invite. --->  {e.Message}");
            throw;
        }
    }

    public async Task<bool> AnyInviteAsync(Guid token, string email, int companyId)
    {
        try
        {
            bool result = await _context.Invites.Where(i => i.CompanyId == companyId)
                .AnyAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error any invite. --->  {e.Message}");
            throw;
        } 
    }

    public async Task<Invite> GetInviteAsync(int inviteId, int companyId)
    {
        try
        {
            Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                .Include(i => i.Company)
                .Include(i => i.Project)
                .Include(i => i.invitor)
                .FirstOrDefaultAsync(i => i.Id == inviteId);

            return invite;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting invite. --->  {e.Message}");
            throw;
        }
    }

    // Params are different which makes the methods different. This is called overloading
    public async Task<Invite> GetInviteAsync(Guid token, string email, int companyId)
    {
        try
        {
            Invite invite = await _context.Invites.Where(i => i.CompanyId == companyId)
                .Include(i => i.Company)
                .Include(i => i.Project)
                .Include(i => i.invitor)
                .FirstOrDefaultAsync(i => i.CompanyToken == token && i.InviteeEmail == email);

            return invite;
        }
        catch (Exception e)
        {
            Console.WriteLine($"****ERROR**** - Error getting invite. --->  {e.Message}");
            throw;
        }
    }

    public async Task<bool> ValidateInviteCodeAsync(Guid? token)
    {
        if (token == null)
        {
            return false;
        }

        bool result = false;

        Invite invite = await _context.Invites.FirstOrDefaultAsync(i => i.CompanyToken == token);

        if (invite != null)
        {
            // Determine the invite date
            DateTime inviteDate = invite.InviteDate.DateTime;
            
            // Custom validation of invite based on the date it was issues
            // In this case we are allowing an invite to be valid for 7 days
            bool validDate = (DateTime.Now - inviteDate).TotalDays <= 7;

            if (validDate)
            {
                result = invite.IsValid;
            }
        }
        return result;
    }
}
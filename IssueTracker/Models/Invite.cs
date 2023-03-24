using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models;

public class Invite
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    [DisplayName("Date Sent")]
    [DataType(DataType.Date)]
    public DateTimeOffset InviteDate { get; set; }
    
    [DisplayName("Date Joined")]
    [DataType(DataType.Date)]
    public DateTimeOffset JoinDate { get; set; }
    
    [DisplayName("Invitee Email")]
    public string InviteeEmail { get; set; }
    
    [DisplayName("Invitee First Name")]
    public string InviteeFirstName { get; set; }
    
    [DisplayName("InviteeLastName")]
    public string InviteeLastName { get; set; }
    
    public bool IsValid { get; set; }

    [DisplayName("Code")]
    // Guid is a globally unique identifier. 
    public Guid CompanyToken { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [DisplayName("Company")]
    public int CompanyId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [DisplayName("Project")]
    public int ProjectId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    // ids that are associated with a user are a string since that's how identity user works
    [DisplayName("invitor")]
    public string InvitorId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    // ids that are associated with a user are a string since that's how identity user works
    [DisplayName("Invitee")]
    public string InviteeId { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual Project Project { get; set; }
    
    public virtual Company Company { get; set; }
    
    public virtual ITUser Invitee { get; set; }
    
    public virtual ITUser invitor { get; set; }
}
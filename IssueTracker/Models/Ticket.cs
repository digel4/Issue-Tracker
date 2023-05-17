using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models;

public class Ticket
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    [Required]
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    [DisplayName("Title ")]
    public string? Title { get; set; }

    [Required]
    [DisplayName("Description")]
    public string Description { get; set; }
    
    [DisplayName("Date Created")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }
    
    [DisplayName("Date Updated")]
    [DataType(DataType.Date)]
    public DateTimeOffset? Updated { get; set; }
    
    [Required]
    [DisplayName("Archived")]
    public bool Archived { get; set; }
    
    [Required]
    [DisplayName("Archived by Project")]
    public bool ArchivedByProject { get; set; }
    
    // This is a foreign key in the database. This corresponds to another table on the database. 
    [Required]
    [DisplayName("Project")]
    public int ProjectId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database. This corresponds to another table on the database. This refers to a  look up table since we are literally just looking for a few properties.
    [DisplayName("Ticket Type")]
    public int TicketTypeId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database. This corresponds to another table on the database. This refers to a  look up table since we are literally just looking for a few properties.
    [DisplayName("Ticket Priority")]
    public int TicketPriorityId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database
    [DisplayName("Ticket Status")]
    public int? TicketStatusId { get; set; }
    
    // This is a foreign key in the database. This corresponds to another table on the database
    [DisplayName("Ticket Owner")]
    public string? OwnerUserId { get; set; }
    
    // This is a foreign key in the database. This corresponds to another table on the database
    [DisplayName("Ticket Developer")]
    public string? DeveloperUserId { get; set; }

    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual ITUser? OwnerUser { get; set; }
    
    public virtual ITUser? DeveloperUser { get; set; }
    
    public virtual Project? Project { get; set; }
    
    public virtual TicketStatus? TicketStatus { get; set; }
    
    public virtual TicketPriority? TicketPriority { get; set; }
    
    public virtual TicketType? TicketType { get; set; }
    
    // These are one-to-many relationship tables. 

    public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

    public virtual ICollection<TicketComment> Comments { get; set; }= new HashSet<TicketComment>();
    
    public virtual ICollection<Notification> Notifications { get; set; }= new HashSet<Notification>();

    public virtual ICollection<TicketHistory> History { get; set; }= new HashSet<TicketHistory>();
    

    

}
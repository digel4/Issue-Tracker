using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IssueTracker.Models.Enums;


namespace IssueTracker.Models;

public class Notification
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    [Required]
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    [DisplayName("Ticket Title")]
    public string Title { get; set; }
    
    [Required]
    [DisplayName("Ticket Message")]
    public string Message { get; set; }
    
    public int NotificationTypeId { get; set; } = default!;
    public NotificationType NotificationType => (NotificationType)NotificationTypeId;
    
    [DisplayName("Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }
    
    public string TimeSinceCreated
    {
        get
        {
            TimeSpan timeSince = DateTime.Now - Created;

            if (timeSince.Days > 30)
                return "Over a month ago";

            if (timeSince.Days > 1)
                return $"{timeSince.Days} days ago";

            if (timeSince.Days == 1)
                return $"Yesterday";

            if (timeSince.Hours > 1)
                return $"{timeSince.Hours} hours ago";

            if (timeSince.Minutes > 1)
                return $"{timeSince.Minutes} minutes ago";

            return "Just now";
        }
    }
    
    [DisplayName("Has been viewed")]
    public bool Viewed { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [DisplayName("Ticket")]
    public int? TicketId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Project")]
    public int ProjectId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Recipent")]
    public string RecipentId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Sender")]
    public string SenderId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Company")]
    public int CompanyId { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual Ticket Ticket { get; set; }
    
    public virtual ITUser Sender { get; set; }
    
    public virtual ITUser Recipent { get; set; }
    
    public virtual Project Project { get; set; }
    
    public virtual Company Company { get; set; }
    

}
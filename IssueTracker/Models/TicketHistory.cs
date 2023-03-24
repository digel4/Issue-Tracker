using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace IssueTracker.Models;

public class TicketHistory
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }

    // This is a foreign key in the database. This is a primary key for a ticket. This is how the two tables will be related
    [DisplayName("Ticket")]
    public int TicketId { get; set; }

    [DisplayName("Updated Item")]
    public string Property { get; set; }
    
    [DisplayName("Previous")]
    public string OldValue { get; set; }
    
    [DisplayName("Current")]
    public string NewValue { get; set; }
    
    [DisplayName("Date Modified")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }
        
    [DisplayName("Description of Change")]
    public string Description { get; set; }
    
    // This is a foreign key in the database. This is a primary key for a user. This is how the two tables will be related
    [DisplayName("Team Member")]
    public string UserId { get; set; }  
    
    // Virtuals
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database
    public virtual Ticket Ticket { get; set; }
    
    public virtual ITUser User { get; set; }
}
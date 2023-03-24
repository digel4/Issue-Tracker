using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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
    
    [DisplayName("Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }
    
    [DisplayName("Has been viewed")]
    public bool Viewed { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [DisplayName("Ticket")]
    public int? TicketId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Recipent")]
    public string RecipentId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database.
    [Required]
    [DisplayName("Sender")]
    public string SenderId { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual Ticket Ticket { get; set; }
    
    public virtual ITUser Sender { get; set; }
    
    public virtual ITUser Recipent { get; set; }
    

}
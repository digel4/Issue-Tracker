using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IssueTracker.Extensions;

namespace IssueTracker.Models;

public class TicketAttachment
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    // This is a foreign key in the database. This is a primary key for a ticket. This is how the two tables will be related. This corresponds with the ticket property below
    [DisplayName("Ticket")]
    public int TicketId { get; set; }
    
    // This is a foreign key in the database. This is a primary key for a user. This is how the two tables will be related. This corresponds with the user property below
    [DisplayName("Team Member")]
    public string? UserId { get; set; }  
    
    [DisplayName("File Description")]
    public string Description { get; set; }
    
    [DisplayName("File Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset Created { get; set; }

    [NotMapped]
    [DisplayName("Select a file")]
    [DataType(DataType.Upload)]
    [MaxFileSize(1024 * 1024)]
    [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" } )]
    public IFormFile FormFile { get; set; }

    [DisplayName("File Name")]
    public string? FileName { get; set; }
    
    public byte[]? FileData { get; set; }
    
    [DisplayName("File Extension")]
    public string? FileContentType { get; set; }
    
    // Virtuals
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database
    public virtual Ticket? Ticket { get; set; }
    
    public virtual ITUser? User { get; set; }
}
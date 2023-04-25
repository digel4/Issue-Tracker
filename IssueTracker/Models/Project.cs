using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace IssueTracker.Models;

public class Project
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    [Required]
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    [DisplayName("Project Name ")]
    public string? Name { get; set; }
    
    [StringLength(400,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    [DisplayName("Description ")]
    public string? Description { get; set; }
    
    [DisplayName("Start Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset StartDate { get; set; }
    
    [DisplayName("End Date")]
    [DataType(DataType.Date)]
    public DateTimeOffset EndDate { get; set; }
    
    [Required]
    [DisplayName("Archived")]
    public bool Archived { get; set; }
    
    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? FormFile { get; set; }

    [DisplayName("File Name")]
    public string? FileName { get; set; }
    
    public byte[]? FileData { get; set; }
    
    [DisplayName("File Extension")]
    public string? FileContentType { get; set; }

    // This is a foreign key in the database.  This corresponds to another table on the database. This corresponds to another table on the database. This refers to a  look up table since we are literally just looking for a few properties.
    [DisplayName("Company")]
    public int? CompanyId { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database. This refers to a  look up table since we are literally just looking for a few properties.
    [DisplayName("Priority")]
    public int? ProjectPriorityId { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual Company Company { get; set; }
    
    public virtual ProjectPriority ProjectPriority { get; set; }
    
    // These are one-to-many relationship tables. 

    public virtual ICollection<Ticket> Tickets { get; set; }= new HashSet<Ticket>();

    // These are many-to-many relationship tables. This join table is automatically made. We don't have to create a Members class
    public virtual ICollection<ITUser> Members { get; set; }= new HashSet<ITUser>();
}
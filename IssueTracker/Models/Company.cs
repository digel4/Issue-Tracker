using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Models;

public class Company
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }
    
    [DisplayName("Company Name")] 
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    public string Name { get; set; }
    
    [DisplayName("Company Description")] 
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    public string Description { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    // These are one-to-many relationship tables. 

    public virtual ICollection<Project> Projects { get; set; }= new HashSet<Project>();

    public virtual ICollection<ITUser> Members { get; set; }= new HashSet<ITUser>();
    
    // Create relationship table to Invites
    
}
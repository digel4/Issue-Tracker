using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Models;

// Standard inhereitence syntax. ITUser is inhereiting from the IdentityUser class
public class ITUser : IdentityUser
{
    // Required is saying it is neccesary to write the record into the db. These data annotations help write the database
    [Required]
    [Display(Name = "First Name ")]
    [StringLength(50,ErrorMessage = "The {0} must be at least {2} and a max {1} characters."),MinLength(2)]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a max {1} characters."), MinLength(2)]
    public string LastName { get; set; }
    
    //This tells the database to ignore this variables. This is also called a decorator.
    [NotMapped]
    // Getters and Setters allow the public values to be got or set. e.g. in this one we're telling runtime how to set the full name by combining the first and last name with string interpolation
    // Sometimes you might only set a get so that it can only be got and not set. Here the application is getting FullName from combining First and LastName.
    public string? FullName { get { return $"{FirstName } {LastName}"; } }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile AvatarFormFile { get; set; }

    [Display(Name= "Avatar")]
    public string? AvatarFileName { get; set; }
    
    public byte[]? AvatarFileData { get; set; }
    
    [Display(Name = "File Extension")]
    public string? AvatarContentType { get; set; }
    
    // This is a foreign key in the database.  This corresponds to another table on the database. This corresponds to another table on the database. This refers to a  look up table since we are literally just looking for a few properties.
    public int? CompanyId { get; set; }
    
    // Virtuals
    // When entity framework sees virtuals it allows easy loading and change tracking. This is opposed to eager loading. Essentially we get the foreign keys above and then the virtual objects below
    // Navigations properties. These allows us to specify the relationships of one class to another. IE we need to specify these to create the relations between the tables in the database.
    // These are not stored in the database
    
    public virtual Company Company { get; set; }
    
    // This is a one-to-many relationship tables. 

    public virtual ICollection<Project> Projects { get; set; }
    
    // public virtual ICollection<Notification> Notifications { get; set; }
}
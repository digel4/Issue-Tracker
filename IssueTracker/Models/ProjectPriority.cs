using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models;


public class ProjectPriority
{
    // This is a look up table
    // This operates as the primary key for identity framework
    public int Id { get; set; }

    [DisplayName("Priority Name")]
    public string Name { get; set; }
}
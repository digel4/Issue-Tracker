using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models;

public class TicketPriority
{
    // This operates as the primary key for identity framework
    public int Id { get; set; }

    [DisplayName("Priority Name")]
    public string Name { get; set; }
}
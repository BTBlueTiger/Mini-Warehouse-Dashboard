using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MiniWarehouse.Api.Model;

public class User
{
    [Key] // Indicates that this property is the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Name { get; set; } = null;
    public string? PhoneNumber { get; set; } = null;
    public DateTime? LastInteraction { get; set; } = null;

}
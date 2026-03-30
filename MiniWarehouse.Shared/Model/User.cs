using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniWarehouse.Shared.Model;

public class User
{
    [Key] // Indicates that this property is the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime? LastInteraction { get; set; } = null;
    public int Role { get; set; } = 0; // 0: Regular User, 1: Admin

}
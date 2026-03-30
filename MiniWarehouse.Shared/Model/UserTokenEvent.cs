using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MiniWarehouse.Shared.Types;

namespace MiniWarehouse.Shared.Model;
public class UserTokenEvent
{
    [Key] // Indicates that this property is the primary key
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; } = null!;
    public required string Token { get; set; }
    public required UserTokenEvents_t Type { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}

using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public DbSet<User> User { get; set; } = default!;
    public DbSet<UserTokenEvent> UserTokenEvent { get; set; } = default!;
}
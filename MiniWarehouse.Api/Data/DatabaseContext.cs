using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Model;

namespace MiniWarehouse.Api.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public DbSet<User> User { get; set; } = default!;
}
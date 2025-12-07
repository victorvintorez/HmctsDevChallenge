using Microsoft.EntityFrameworkCore;
using Task = HmctsDevChallenge.Backend.Models.Database.Task;

namespace HmctsDevChallenge.Backend.Database;

public class HmctsDbContext(DbContextOptions options) : DbContext(options)
{
    public virtual DbSet<Task> Tasks { get; set; }
}
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<Organisation> Organisations=>Set<Organisation>();
        public DbSet<Project> Projects=>Set<Project>();
        public DbSet<User> Users=>Set<User>();
        public DbSet<TaskItem> Tasks=>Set<TaskItem>();
    }
}
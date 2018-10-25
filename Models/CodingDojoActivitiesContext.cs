using Microsoft.EntityFrameworkCore;

namespace CodingDojoActivities.Models
{
    public class CodingDojoActivitiesContext : DbContext
    {
        public DbSet<User> Users {get;set;}
        public DbSet<cdActivity> cdActivities {get;set;}
        public DbSet<Participation> Participations {get;set;}
        public CodingDojoActivitiesContext(DbContextOptions<CodingDojoActivitiesContext> options) : base(options) {}
    }
}
using Microsoft.EntityFrameworkCore;
using DatingAppAPI.Models;
namespace DatingAppAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base
        (options){}
        public DbSet<Value> Values { get; set; }
    }
}
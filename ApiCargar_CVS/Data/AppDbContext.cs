using ApiCargar_CVS.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCargar_CVS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
    }
}

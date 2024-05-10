using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Data.Contexts
{
    public class DataContext : IdentityDbContext<UserAccount>

    {
        public DataContext(DbContextOptions options) : base(options)
        {
           
        }

        public DbSet<AddressEntity> UserAddresses { get; set; }
    }
}

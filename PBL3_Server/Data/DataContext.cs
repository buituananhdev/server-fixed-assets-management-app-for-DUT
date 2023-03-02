global using Microsoft.EntityFrameworkCore;

namespace PBL3_Server.Data
{
    public class DataContext : DbContext
    {
        

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=PBL3_fixed_assets_management;Trusted_Connection=true;TrustServerCertificate=true");
        }

        public DbSet<Asset> Assets { get; set; }

        public DbSet<DisposedAsset> DisposedAssets { get; set; }
    }
}

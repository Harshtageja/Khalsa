using MedicalStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace MedicalStore.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


            
        }
        public DbSet<StoreKeeper> StoreKeepers { get; set; }
        public DbSet<StoreManager> StoreManagers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Pending> Pendings { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<cart> Cart { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderShown> OrdersShown { get; set; }
        public DbSet<cartWithMedicineName> cartWithMedicineNames { get; set; }


    }
}

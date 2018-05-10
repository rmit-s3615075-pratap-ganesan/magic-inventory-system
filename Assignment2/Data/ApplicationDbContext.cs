using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Assignment2.Models;

namespace Assignment2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
      
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OwnerInventory> OwnerInventory { get; set; }
        public DbSet<StockRequest> StockRequest { get; set; }
        public DbSet<StoreInventory> StoreInventory { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
        }
    }
}

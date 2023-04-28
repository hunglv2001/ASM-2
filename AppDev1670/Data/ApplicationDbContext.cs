using AppDev1670.Models;
using AppDev1670.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AppDev1670.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            this.SeedRoles(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Id = "fab4fac1-c546-41de-aebc-a14da6895711",
                Name = Role.CUSTOMER,
                ConcurrencyStamp = "1",
                NormalizedName = Role.CUSTOMER
            },
            new IdentityRole()
            {
                Id = "c7b013f0-5201-4317-abd8-c211f91b7330",
                Name = Role.ADMIN,
                ConcurrencyStamp = "2",
                NormalizedName = Role.ADMIN
            },
            new IdentityRole()
            {
                Id = "0d3151ac-ebae-47d5-8bd1-ac783afafee2",
                Name = Role.STORE_OWNER,
                ConcurrencyStamp = "3",
                NormalizedName = Role.STORE_OWNER
            }
            );
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { set; get; }
        public DbSet<Order> Orders { set; get; }
        public DbSet<OrderDetail> OrderDetails { set; get; }

    }
}

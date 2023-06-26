using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api_aspnetcore6.Models
{
    public partial class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Category>? Categories { get; set; }
        public virtual DbSet<Product>? Products { get; set; }
        public virtual DbSet<Order>? Orders { get; set; }
        public virtual DbSet<OrderDetail>? OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(d => d.Order)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(d => d.IdOrder)
            .HasConstraintName("FK_IdOrder_OrderDetail");

                entity.HasOne(d => d.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(d => d.IdProduct)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_IdProduct_OrderDetail");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

}
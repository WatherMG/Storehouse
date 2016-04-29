namespace Storehouse
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dbModel : DbContext
    {
        public dbModel()
            : base("name=dbModel")
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Expire> Expires { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Temperature> Temperatures { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Category)
                .HasForeignKey(e => e.category_id);

            modelBuilder.Entity<Expire>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Expire)
                .HasForeignKey(e => e.expire_date_id);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Manufacturer>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Manufacturer)
                .HasForeignKey(e => e.manufacturer_id);

            modelBuilder.Entity<Product>()
                .Property(e => e.price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Invoices)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<Supplier>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Supplier)
                .HasForeignKey(e => e.supplier_id);

            modelBuilder.Entity<Temperature>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Temperature)
                .HasForeignKey(e => e.temperature_id);
        }
    }
}

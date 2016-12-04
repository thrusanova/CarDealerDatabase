namespace CarDealers.Data
{
    using Data;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CarDealersContext : DbContext
    {

        public CarDealersContext()
            : base("name=CarDealersContext")
        {
        }
        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Sale> Sales { get; set; }

        public virtual DbSet<Supplier> Suppliers { get; set; }

        public virtual DbSet<Part> Parts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Properties<DateTime>()
.Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Part>()
                .HasMany(p => p.Cars)
                .WithMany(c=>c.Parts)
                .Map(u =>
                {
                    u.MapLeftKey("Part_Id");
                    u.MapRightKey("Car_Id");
                    u.ToTable("PartCars");
                });
       
            base.OnModelCreating(modelBuilder);
        }
    }
}
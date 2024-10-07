using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities
{
    public partial class KoiDbContext : DbContext
    {
        public KoiDbContext()
        {
        }

        public KoiDbContext(DbContextOptions<KoiDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Airport> Airports { get; set; }
        public virtual DbSet<Breed> Breeds { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Koi> Kois { get; set; }
        public virtual DbSet<KoiBreed> KoiBreeds { get; set; }
        public virtual DbSet<KoiFarm> KoiFarms { get; set; }
        public virtual DbSet<KoiImage> KoiImages { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderStorage> OrderStorages { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<RefundRequest> RefundRequests { get; set; }
        public virtual DbSet<RefundRequestMedium> RefundRequestMedia { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ShippingFee> ShippingFees { get; set; }
        public virtual DbSet<StorageProvince> StorageProvinces { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("Server=localhost;uid=SA;pwd=12345;database=KoiDB;TrustServerCertificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var airportId1 = Guid.NewGuid();
            var airportId2 = Guid.NewGuid();
            var flightId1 = Guid.NewGuid();
            var breedId1 = Guid.NewGuid();
            var breedId2 = Guid.NewGuid();
            var koiId1 = Guid.NewGuid();
            var cartId1 = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var koiFarmId1 = Guid.NewGuid();
            var roleAdminId = Guid.NewGuid();
            var roleUserId = Guid.NewGuid();
            var orderId1 = Guid.NewGuid();
            var orderStorageId1 = Guid.NewGuid();
            var RefundRequestId = Guid.NewGuid();
            var policyId = Guid.NewGuid();
            var storageProvinceId = Guid.NewGuid();

            // Airport Entity
            modelBuilder.Entity<Airport>(entity =>
            {
                entity.HasKey(e => e.AirportId);
                entity.Property(e => e.AirportId).ValueGeneratedNever();
                entity.Property(e => e.AirportName).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(100);

                entity.HasMany(e => e.FlightArrivalAirports)
                    .WithOne(f => f.ArrivalAirport)
                    .HasForeignKey(f => f.ArrivalAirportId)
                    .OnDelete(DeleteBehavior.NoAction);  //.NoAction delete for arrival flights

                entity.HasMany(e => e.FlightDepartureAirports)
                    .WithOne(f => f.DepartureAirport)
                    .HasForeignKey(f => f.DepartureAirportId)
                    .OnDelete(DeleteBehavior.NoAction);  //.NoAction delete for departure flights

                // Seed data
                entity.HasData(
                    new Airport { AirportId = airportId1, AirportName = "Tokyo International Airport", Address = "12333" },
                    new Airport { AirportId = airportId2, AirportName = "Los Angeles International Airport", Address = "!233123132" }
                );
            });

            // Flight Entity
            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.FlightId);
                entity.Property(e => e.FlightId).ValueGeneratedNever();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.ArrivalAirport)
                    .WithMany(p => p.FlightArrivalAirports)
                    .HasForeignKey(d => d.ArrivalAirportId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(d => d.DepartureAirport)
                    .WithMany(p => p.FlightDepartureAirports)
                    .HasForeignKey(d => d.DepartureAirportId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Seed data
                entity.HasData(
                    new Flight { FlightId = flightId1, Code = "FL123", Status = "On Time", DepartureAirportId = airportId1, ArrivalAirportId = airportId2 }
                );
            });

            // Breed Entity
            modelBuilder.Entity<Breed>(entity =>
            {
                entity.HasKey(e => e.BreedId);
                entity.Property(e => e.BreedId).ValueGeneratedNever();
                entity.Property(e => e.Name).HasMaxLength(100);

                // Seed data
                entity.HasData(
                    new Breed { BreedId = breedId1, Name = "Kohaku" },
                    new Breed { BreedId = breedId2, Name = "Sanke" }
                );
            });

            // Koi Entity
            modelBuilder.Entity<Koi>(entity =>
            {
                entity.HasKey(e => e.KoiId);
                entity.Property(e => e.KoiId).ValueGeneratedNever();
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.AvatarLink).HasMaxLength(100);
                entity.Property(e => e.CertificationLink).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(100).IsRequired(false);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                // Seed data
                entity.HasData(
                    new Koi { KoiId = koiId1, Name = "Koi A", AvatarLink = "avatar_link_1", CertificationLink = "certification_link_1", Gender = "Male", Dob = DateTime.Now.AddYears(-1), Price = 5000, FarmId = koiFarmId1 }
                );
            });

            // KoiFarm Entity
            modelBuilder.Entity<KoiFarm>(entity =>
            {
                entity.HasKey(e => e.KoiFarmId);
                entity.Property(e => e.KoiFarmId).ValueGeneratedNever();
                entity.Property(e => e.FarmName).HasMaxLength(50);
                entity.Property(e => e.FarmAddress).HasMaxLength(200);
                entity.Property(e => e.FarmDescription).HasMaxLength(50);

                // Seed data
                entity.HasData(
                    new KoiFarm { KoiFarmId = koiFarmId1, KoiFarmManagerId = userId1, FarmName = "Koi Farm 1", FarmAddress = "123 Farm Rd", FarmDescription = "High-quality Koi farm", FarmAvatar = "farm_avatar_link" }
                );
            });

            // Order Entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).ValueGeneratedNever();
                entity.Property(e => e.OrderNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(100);
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 0)");

                entity.HasOne(o => o.Customer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.NoAction);  //.NoAction delete for orders when a user is deleted

                // Seed data
                entity.HasData(
                    new Order
                    {
                        OrderId = orderId1,
                        CustomerId = userId1,
                        PolicyId = policyId,
                        CreatedDate = DateTime.Now,
                        OrderNumber = "ORD123",
                        TotalPrice = 10000,
                        Status = "Processing",
                        Address = "123 Main St",
                        Phone = "1234567890",
                        ShippingFee = "1111"
                    }
                );
            });

            // OrderStorage Entity
            modelBuilder.Entity<OrderStorage>(entity =>
            {
                entity.HasKey(e => e.OrderStorageId);
                entity.Property(e => e.OrderStorageId).ValueGeneratedNever();
                entity.Property(e => e.ArrivalTime).HasColumnType("datetime");

                // Seed data
                entity.HasData(
                    new OrderStorage { OrderStorageId = Guid.NewGuid(), StorageProvinceId = storageProvinceId, OrderId = orderId1, ShipperId = userId1, ArrivalTime = DateTime.Now }
                );
            });

            // Policy Entity
            modelBuilder.Entity<Policy>(entity =>
            {
                entity.HasKey(e => e.PolicyId);
                entity.Property(e => e.PolicyId).ValueGeneratedNever();
                entity.Property(e => e.PolicyName).HasMaxLength(100);

                // Seed data
                entity.HasData(
                    new Policy { PolicyId = policyId, PolicyName = "Refund Policy", FarmId = koiFarmId1, Description = "Refund Policy" }
                );
            });

            // RefreshToken Entity
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.RefreshTokenId);
                entity.Property(e => e.RefreshTokenId).ValueGeneratedNever();
                entity.Property(e => e.ExpiredAt).HasColumnType("datetime");

                // Seed data
                entity.HasData(
                    new RefreshToken { RefreshTokenId = Guid.NewGuid(), UserId = userId1, RefreshToken1 = "abcd1234", JwtId = "abcd1234", ExpiredAt = DateTime.Now.AddDays(30) }
                );
            });

            // RefundRequest Entity
            modelBuilder.Entity<RefundRequest>(entity =>
            {
                entity.HasKey(e => e.RefundRequestId);
                entity.Property(e => e.RefundRequestId).ValueGeneratedNever();
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50);

                // Seed data
                entity.HasData(
                    new RefundRequest { RefundRequestId = RefundRequestId, OrderId = orderId1, CreatedDate = DateTime.Now, Status = "Pending", Description = "Pending", Response = "Pending" }
                );
            });

            // RefundRequestMedium Entity
            modelBuilder.Entity<RefundRequestMedium>(entity =>
            {
                entity.HasKey(e => e.RefundRequestMediaId);
                entity.Property(e => e.RefundRequestMediaId).ValueGeneratedNever();

                // Seed data
                entity.HasData(
                    new RefundRequestMedium { RefundRequestMediaId = Guid.NewGuid(), RefundRequestId = RefundRequestId, Link = "Pending" }
                );
            });

            // Role Entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleId).ValueGeneratedNever();
                entity.Property(e => e.RoleName).HasMaxLength(100);

                // Seed data
                entity.HasData(
                    new Role { RoleId = roleAdminId, RoleName = "Admin" },
                    new Role { RoleId = roleUserId, RoleName = "User" }
                );
            });

            // ShippingFee Entity
            modelBuilder.Entity<ShippingFee>(entity =>
            {
                entity.HasKey(e => e.ShippingFeeId);
                entity.Property(e => e.ShippingFeeId).ValueGeneratedNever();
                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

                // Configuring relationships
                entity.HasOne(d => d.StorageProvinceVn)
                    .WithMany(p => p.ShippingFeeStorageProvinceVns)
                    .HasForeignKey(d => d.StorageProvinceVnId)
                    .OnDelete(DeleteBehavior.NoAction);  //.NoAction delete for shipping fees

                entity.HasOne(d => d.StorageProvinceJp)
                    .WithMany(p => p.ShippingFeeStorageProvinceJps)
                    .HasForeignKey(d => d.StorageProvinceJpId)
                    .OnDelete(DeleteBehavior.NoAction);  //.NoAction delete for shipping fees
            });

            // StorageProvince Entity
            modelBuilder.Entity<StorageProvince>(entity =>
            {
                entity.HasKey(e => e.StorageProvinceId);
                entity.Property(e => e.StorageProvinceId).ValueGeneratedNever();
                entity.Property(e => e.Country).HasMaxLength(100);
                entity.Property(e => e.ProvinceName).HasMaxLength(100);

                // Seed data
                entity.HasData(
                    new StorageProvince { StorageProvinceId = storageProvinceId, Country = "Vietnam", ProvinceName = "Hanoi", StorageName = "Hanoi Storage", AirportId = airportId1, Address = "Pending" }
                );
            });

            // Transaction Entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.TransactionId).ValueGeneratedNever();
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                // Seed data
                entity.HasData(
                    new Transaction { TransactionId = Guid.NewGuid(), OrderId = orderId1, CreatedDate = DateTime.Now, Status = "Success", PaymentMethodTransaction = "Credit Card", TransactionInfo = "111", TransactionNumber = "!11" }
                );
            });

            // User Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).ValueGeneratedNever();
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(200).IsRequired();
                entity.Property(e => e.AvatarLink).HasMaxLength(200).IsRequired();
                entity.Property(e => e.UserName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Salt).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();

                // Seed data
                entity.HasData(
                    new User
                    {
                        UserId = userId1,
                        UserName = "john_doe",
                        FullName = "John Doe",
                        Email = "johndoe@example.com",
                        Gender = "Male",
                        RoleId = roleAdminId,
                        Address = "123 Main St, City, Country",
                        AvatarLink = "https://example.com/avatar/john_doe.jpg",
                        Phone = "1234567890",
                        Salt = Convert.FromBase64String("c2FsdFZhbHVl"),
                        PasswordHash = Convert.FromBase64String("cGFzc3dvcmRIYXNoVmFsdWU="),
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Status = true
                    },
                    new User
                    {
                        UserId = userId2,
                        UserName = "jane_doe",
                        FullName = "Jane Doe",
                        Email = "janedoe@example.com",
                        Gender = "Female",
                        RoleId = roleUserId,
                        Address = "456 Main St, City, Country",
                        AvatarLink = "https://example.com/avatar/jane_doe.jpg",
                        Phone = "0987654321",
                        Salt = Convert.FromBase64String("c2FsdFZhbHVl"),
                        PasswordHash = Convert.FromBase64String("cGFzc3dvcmRIYXNoVmFsdWU="),
                        DateOfBirth = new DateTime(1992, 2, 2),
                        Status = true
                    }
                );
            });

             // Cart Entity
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId);
                entity.Property(e => e.CartId).ValueGeneratedNever();
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.HasOne(ci => ci.User)
                    .WithMany(k => k.Carts)
                    .HasForeignKey(ci => ci.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Seed data
                entity.HasData(
                    new Cart { CartId = cartId1, UserId = userId1, CreatedDate = DateTime.Now }
                );
            });

             modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.CartItemId);

                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.NoAction); // NoAction delete when a cart is deleted

                entity.HasOne(ci => ci.Koi)
                    .WithMany(k => k.CartItems)
                    .HasForeignKey(ci => ci.KoiId)
                    .OnDelete(DeleteBehavior.NoAction); // NoAction delete when a Koi is deleted

                // Seed data
                entity.HasData(
                    new CartItem { CartItemId = Guid.NewGuid(), CartId = cartId1, KoiId = koiId1, Quantity = 1, UnitPrice = 5000M }
                );
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

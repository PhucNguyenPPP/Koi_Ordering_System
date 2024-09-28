using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

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

    public virtual DbSet<FarmImage> FarmImages { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Koi> Kois { get; set; }

    public virtual DbSet<KoiImage> KoiImages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStorage> OrderStorages { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RefundRequest> RefundRequests { get; set; }

    public virtual DbSet<RefundRequestMedium> RefundRequestMedia { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;uid=SA;pwd=12345;database=KoiDB;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__Airport__E3DBE0EA8CB6011F");

            entity.ToTable("Airport");

            entity.Property(e => e.AirportId).ValueGeneratedNever();
            entity.Property(e => e.AirportName).HasMaxLength(100);
        });

        modelBuilder.Entity<Breed>(entity =>
        {
            entity.HasKey(e => e.BreedId).HasName("PK__Breed__D1E9AE9DE294007A");

            entity.ToTable("Breed");

            entity.Property(e => e.BreedId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<FarmImage>(entity =>
        {
            entity.HasKey(e => e.FarmImageId).HasName("PK__FarmImag__B6C325794D4C4DA6");

            entity.ToTable("FarmImage");

            entity.Property(e => e.FarmImageId).ValueGeneratedNever();

            entity.HasOne(d => d.Farm).WithMany(p => p.FarmImages)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FarmImage__FarmI__33D4B598");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__Flight__8A9E14EE2C597469");

            entity.ToTable("Flight");

            entity.Property(e => e.FlightId).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.ShippingFee).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.ArrivalAirport).WithMany(p => p.FlightArrivalAirports)
                .HasForeignKey(d => d.ArrivalAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__ArrivalA__30F848ED");

            entity.HasOne(d => d.DepartureAirport).WithMany(p => p.FlightDepartureAirports)
                .HasForeignKey(d => d.DepartureAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__Departur__300424B4");
        });

        modelBuilder.Entity<Koi>(entity =>
        {
            entity.HasKey(e => e.KoiId).HasName("PK__Koi__E034359816B85790");

            entity.ToTable("Koi");

            entity.Property(e => e.KoiId).ValueGeneratedNever();
            entity.Property(e => e.Dob).HasColumnType("datetime");
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Breed).WithMany(p => p.Kois)
                .HasForeignKey(d => d.BreedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Koi__BreedId__412EB0B6");

            entity.HasOne(d => d.Farm).WithMany(p => p.Kois)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Koi__FarmId__4316F928");

            entity.HasOne(d => d.Order).WithMany(p => p.Kois)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Koi__OrderId__4222D4EF");
        });

        modelBuilder.Entity<KoiImage>(entity =>
        {
            entity.HasKey(e => e.KoiImageId).HasName("PK__KoiImage__11060AD2FACAD503");

            entity.ToTable("KoiImage");

            entity.Property(e => e.KoiImageId).ValueGeneratedNever();

            entity.HasOne(d => d.Koi).WithMany(p => p.KoiImages)
                .HasForeignKey(d => d.KoiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KoiImage__KoiId__45F365D3");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCFC0171041");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(50);
            entity.Property(e => e.PackingMethod).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__CustomerI__3C69FB99");

            entity.HasOne(d => d.Flight).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("FK__Order__FlightId__3E52440B");

            entity.HasOne(d => d.Policy).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__PolicyId__3D5E1FD2");
        });

        modelBuilder.Entity<OrderStorage>(entity =>
        {
            entity.HasKey(e => e.OrderStorageId).HasName("PK__OrderSto__5DEFA466191C4CCA");

            entity.ToTable("OrderStorage");

            entity.Property(e => e.OrderStorageId).ValueGeneratedNever();
            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStorages)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderStor__Order__49C3F6B7");

            entity.HasOne(d => d.Storage).WithMany(p => p.OrderStorages)
                .HasForeignKey(d => d.StorageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderStor__Stora__48CFD27E");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PK__PaymentM__DC31C1D3B13AD9DB");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentMethodId).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__Policy__2E1339A4E4A8524D");

            entity.ToTable("Policy");

            entity.Property(e => e.PolicyId).ValueGeneratedNever();

            entity.HasOne(d => d.Farm).WithMany(p => p.Policies)
                .HasForeignKey(d => d.FarmId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Policy__FarmId__398D8EEE");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Policies)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Policy__PaymentM__38996AB5");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E391EA599AC");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.RefreshTokenId).ValueGeneratedNever();
            entity.Property(e => e.ExpiredAt).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken1).HasColumnName("RefreshToken");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__5535A963");
        });

        modelBuilder.Entity<RefundRequest>(entity =>
        {
            entity.HasKey(e => e.RefundRequestId).HasName("PK__RefundRe__A67BF229FFA6F72B");

            entity.ToTable("RefundRequest");

            entity.Property(e => e.RefundRequestId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Order).WithMany(p => p.RefundRequests)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefundReq__Order__4CA06362");
        });

        modelBuilder.Entity<RefundRequestMedium>(entity =>
        {
            entity.HasKey(e => e.RefundRequestMediaId).HasName("PK__RefundRe__D856E8F87D62D974");

            entity.Property(e => e.RefundRequestMediaId).ValueGeneratedNever();

            entity.HasOne(d => d.RefundRequest).WithMany(p => p.RefundRequestMedia)
                .HasForeignKey(d => d.RefundRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefundReq__Refun__4F7CD00D");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A5974FC79");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageId).HasName("PK__Storage__8A247E571A65E38B");

            entity.ToTable("Storage");

            entity.Property(e => e.StorageId).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__55433A6BDE80A65E");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransactionId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodTransaction).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(100);
            entity.Property(e => e.TransactionInfo).HasMaxLength(100);
            entity.Property(e => e.TransactionNumber).HasMaxLength(100);

            entity.HasOne(d => d.Order).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__Order__52593CB8");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4CCCAE61BF");

            entity.ToTable("User");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.DateOfBirth).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FarmName).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.OtpExpiredTime).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__RoleId__286302EC");

            entity.HasOne(d => d.Storage).WithMany(p => p.Users)
                .HasForeignKey(d => d.StorageId)
                .HasConstraintName("FK__User__StorageId__29572725");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

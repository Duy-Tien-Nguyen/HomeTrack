using Microsoft.EntityFrameworkCore;
using HomeTrack.Domain;
using HomeTrack.Domain.Account;

namespace HomeTrack.Infrastructure.Data
{
  public class ApplicationDBContext : DbContext
  {
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ConfirmationToken> ConfirmationTokens { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Subscription> Subscriptions{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("Users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
        entity.HasIndex(u => u.Email).IsUnique();
        entity.Property(u => u.Password).IsRequired().HasMaxLength(256);
        entity.Property(u => u.FirstName).HasMaxLength(100);
        entity.Property(u => u.LastName).HasMaxLength(100);
        entity.Property(u => u.Role).IsRequired();
        entity.Property(u => u.Status).IsRequired();
        entity.Property(u => u.CreatedAt).IsRequired();
        entity.Property(u => u.UpdatedAt).IsRequired();

        entity.HasIndex(e => e.Email);
      });

      modelBuilder.Entity<ConfirmationToken>(entity =>
      {
        entity.ToTable("ConfirmationTokens");
        entity.HasKey(ct => ct.Id);
        entity.Property(ct => ct.UserId).IsRequired();
        entity.Property(ct => ct.Token).IsRequired().HasMaxLength(6);
        entity.Property(ct => ct.ExpirationAt).IsRequired();
        entity.Property(ct => ct.Used).IsRequired();

        // Define foreign key relationship
        entity.HasOne<User>()
              .WithMany(u => u.ConfirmationTokens)
              .HasForeignKey(ct => ct.UserId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<Package>(entity =>
      {
        entity.ToTable("Packages");
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
          .IsRequired()
          .HasMaxLength(100);
        entity.HasIndex(e => e.Name)
          .IsUnique();

        entity.Property(e => e.Description)
          .HasMaxLength(500);

        entity.Property(e => e.Price)
          .IsRequired()
          .HasColumnType("decimal(18,2)");

        entity.Property(e => e.DurationDays)
          .IsRequired();

        entity.Property(e => e.isActive)
          .IsRequired()
          .HasDefaultValue(true);

        entity.Property(e => e.CreateAt)
          .IsRequired()
          .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.UpdateAt)
          .IsRequired();
      });

      modelBuilder.Entity<Subscription>(entity =>
      {
        entity.ToTable("Subscriptions");
        entity.HasKey(e => e.Id);

        entity.HasOne(e => e.Package)
          .WithMany(e => e.Subscriptions)
          .HasForeignKey(e => e.PackageId)
          .OnDelete(DeleteBehavior.Restrict);

        entity.Property(e => e.Status)
          .IsRequired();

        entity.Property(e => e.StartsAt)
          .IsRequired();

        entity.Property(e => e.EndsAt)
          .IsRequired();

        entity.Property(e => e.CreatedAt)
          .IsRequired()
          .HasDefaultValueSql("CURRENT_TIMESTAMP");

        entity.Property(e => e.UpdatedAt)
          .IsRequired();

        entity.HasIndex(e => e.UserId);
        entity.HasIndex(s => s.EndsAt);
      });
    }
  }
}
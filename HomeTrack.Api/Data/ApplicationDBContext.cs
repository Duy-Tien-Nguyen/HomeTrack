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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<User>(entity=>
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
      });
      modelBuilder.Entity<ConfirmationToken>(entity=>
      {
        entity.ToTable("ConfirmationTokens");
        entity.HasKey(ct => ct.Id);
        entity.Property(ct => ct.UserId).IsRequired();
        entity.Property(ct => ct.Token).IsRequired().HasMaxLength(256);
        entity.Property(ct => ct.ExpirationAt).IsRequired();
        entity.Property(ct => ct.Used).IsRequired();
        
        // Define foreign key relationship
        entity.HasOne<User>()
              .WithMany(u => u.ConfirmationTokens)
              .HasForeignKey(ct => ct.UserId)
              .OnDelete(DeleteBehavior.Cascade);
      });

      base.OnModelCreating(modelBuilder);
    }
  }
}
using Microsoft.EntityFrameworkCore;
using HomeTrack.Domain;
using HomeTrack.Domain.Account;
using System.Reflection.Emit;

namespace HomeTrack.Infrastructure.Data
{
      public class ApplicationDBContext : DbContext
      {
            public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
            { }

            public DbSet<User> Users { get; set; }
            public DbSet<ConfirmationToken> ConfirmationTokens { get; set; }
            public DbSet<Package> Packages { get; set; }
            public DbSet<Subscription> Subscriptions { get; set; }
            public DbSet<SystemSetting> SystemSettings { get; set; }
            public DbSet<Item> Items { get; set; }
            public DbSet<Location> Locations { get; set; }
    public DbSet<Tag> Tags { get; set; }
            public DbSet<ItemTag> ItemTags { get; set; }
            public DbSet<FileStorage> FileStorages { get; set; }
            public DbSet<Subscription> Subscription { get; set; }
            public DbSet<SearchLog> SearchLogs { get; set; }
            public DbSet<StatsReport> StatsReports { get; set; }

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

                  modelBuilder.Entity<SystemSetting>(entity =>
                  {
                        entity.ToTable("SystemSettings");
                        entity.HasKey(e => e.Id);
                        entity.HasIndex(e => e.SettingKey).IsUnique();
                  });
                  modelBuilder.Entity<SystemSetting>().HasData(
                    new SystemSetting
                    {
                          Id = 1, // Đặt Id cố định để dễ cập nhật
                          SettingKey = "MaxBasicItemLimit",
                          SettingValue = 50, // Giá trị mặc định
                          updateAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new SystemSetting
                    {
                          Id = 2,
                          SettingKey = "MaxPremiumItemLimit",
                          SettingValue = 500,
                          updateAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc)
                    }
                  );
                  modelBuilder.Entity<Item>(entity =>
                 {
                       entity.ToTable("items");
                       entity.HasKey(i => i.Id);

                       entity.Property(i => i.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

                       entity.Property(i => i.Name)
                  .HasColumnName("name")
                  .IsRequired()
                  .HasMaxLength(255);

                       entity.Property(i => i.Description)
                  .HasColumnName("description")
                  .HasColumnType("TEXT");

                       entity.Property(i => i.ImageUrl)
                  .HasColumnName("image_url")
                  .HasMaxLength(2048);

                       entity.Property(i => i.LocationId)
                  .HasColumnName("location_id");

                       entity.Property(i => i.CreatedAt)
                  .HasColumnName("created_at")
                  .IsRequired();

                       entity.Property(i => i.UpdatedAt)
                  .HasColumnName("updated_at")
                  .IsRequired();

                       entity.Property(i => i.DeletedAt)
                  .HasColumnName("deleted_at");

                       entity.HasOne(i => i.User)
                  .WithMany(u => u.Items)
                  .HasForeignKey(i => i.UserId)
                  .HasConstraintName("fk_items_user_id")
                  .OnDelete(DeleteBehavior.Restrict);

                       entity.HasOne(i => i.Location)
                  .WithMany(l => l.Items)
                  .HasForeignKey(i => i.LocationId)
                  .HasConstraintName("fk_items_location_id")
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

                       entity.HasMany(i => i.AttachedFiles)
                  .WithOne(fs => fs.Item)
                  .HasForeignKey(fs => fs.ItemId)
                  .HasConstraintName("fk_filestorages_item_id")
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
                 });

                  modelBuilder.Entity<Location>(entity =>
                  {
                        entity.ToTable("locations");
                        entity.HasKey(l => l.Id);

                        entity.Property(l => l.UserId)
                  .HasColumnName("user_id")
                  .IsRequired();

                        entity.Property(l => l.Name)
                  .HasColumnName("name")
                  .IsRequired()
                  .HasMaxLength(255);

                        entity.Property(l => l.ParentLocationId)
                  .HasColumnName("parent_location_id");

                        entity.Property(l => l.Description)
                  .HasColumnName("description")
                  .HasColumnType("TEXT");

                        entity.Property(l => l.CreatedAt)
                  .HasColumnName("created_at")
                  .IsRequired();

                        entity.Property(l => l.UpdatedAt)
                  .HasColumnName("updated_at")
                  .IsRequired();

                        entity.HasOne(l => l.User)
                  .WithMany(u => u.Locations)
                  .HasForeignKey(l => l.UserId)
                  .HasConstraintName("fk_locations_user_id")
                  .OnDelete(DeleteBehavior.Restrict);

                        entity.HasOne(l => l.ParentLocation)
                  .WithMany(p => p.ChildLocations)
                  .HasForeignKey(l => l.ParentLocationId)
                  .HasConstraintName("fk_locations_parent_location_id")
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
                  });

                  modelBuilder.Entity<Tag>(entity =>
                  {
                        entity.ToTable("tags");
                        entity.HasKey(t => t.Id);

                        entity.Property(t => t.Name)
                  .HasColumnName("name")
                  .IsRequired()
                  .HasMaxLength(100);

                        entity.HasIndex(t => t.Name)
                  .IsUnique()
                  .HasDatabaseName("ix_tags_name_unique");
                        entity.Property(t => t.UserId)
                  .HasColumnName("user_id");

                        entity.Property(t => t.CreatedAt)
                  .HasColumnName("created_at")
                  .IsRequired();

                        entity.HasOne(t => t.User)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(t => t.UserId)
                  .HasConstraintName("fk_tags_user_id")
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
                  });

                  modelBuilder.Entity<ItemTag>(entity =>
                  {
                        entity.ToTable("item_tags");

                        entity.HasKey(it => new { it.ItemId, it.TagId })
                  .HasName("pk_item_tags");

                        entity.Property(it => it.ItemId)
                  .HasColumnName("item_id");

                        entity.Property(it => it.TagId)
                  .HasColumnName("tag_id");

                        entity.HasOne(it => it.Item)
                  .WithMany(i => i.ItemTags)
                  .HasForeignKey(it => it.ItemId)
                  .HasConstraintName("fk_itemtags_item_id")
                  .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(it => it.Tag)
                  .WithMany(t => t.ItemTags)
                  .HasForeignKey(it => it.TagId)
                  .HasConstraintName("fk_itemtags_tag_id")
                  .OnDelete(DeleteBehavior.Cascade);
                  });

                  modelBuilder.Entity<FileStorage>(entity =>
                  {
                        entity.ToTable("file_storages");
                        entity.HasKey(fs => fs.Id);

                        entity.Property(fs => fs.FileName)
                  .HasColumnName("file_name")
                  .IsRequired()
                  .HasMaxLength(255);

                        entity.Property(fs => fs.FilePath)
                  .HasColumnName("file_path")
                  .IsRequired()
                  .HasMaxLength(1024);

                        entity.Property(fs => fs.ContentType)
                  .HasColumnName("content_type")
                  .HasMaxLength(100);

                        entity.Property(fs => fs.FileSize)
                  .HasColumnName("file_size");

                        entity.Property(fs => fs.UserId)
                  .HasColumnName("user_id");

                        entity.Property(fs => fs.ItemId)
                  .HasColumnName("item_id");

                        entity.Property(fs => fs.UploadedAt)
                  .HasColumnName("uploaded_at")
                  .IsRequired();

                        entity.HasOne(fs => fs.User)
                  .WithMany(u => u.FileStorages)
                  .HasForeignKey(fs => fs.UserId)
                  .HasConstraintName("fk_filestorages_user_id")
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
                  });
            }
  }
}
using Microsoft.EntityFrameworkCore;
using HomeTrack.Domain;
using HomeTrack.Domain.Account;
using HomeTrack.Api.Models.Entities;

namespace HomeTrack.Infrastructure.Data
{
  public class ApplicationDBContext : DbContext
  {
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ConfirmationToken> ConfirmationTokens { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ItemTag> ItemTags { get; set; }
    public DbSet<FileStorage> FileStorages { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Subscription> Subscriptions{ get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("users");
        entity.HasKey(u => u.Id);

        entity.Property(u => u.Email)
              .IsRequired().HasMaxLength(256);

        entity.HasIndex(u => u.Email)
              .IsUnique();

        entity.Property(u => u.Password)
              .HasColumnName("password")
              .IsRequired()
              .HasMaxLength(256);

        entity.Property(u => u.FirstName)
              .HasColumnName("first_name")
              .HasMaxLength(100);

        entity.Property(u => u.LastName)
              .HasColumnName("last_name")
              .HasMaxLength(100);

        entity.Property(u => u.Role)
              .HasColumnName("role")
              .IsRequired()
              .HasConversion<string>()
              .HasMaxLength(50);

        entity.Property(u => u.Status)
              .HasColumnName("status")
              .IsRequired()
              .HasConversion<string>()
              .HasMaxLength(50);

        entity.Property(u => u.RefreshToken)
              .HasColumnName("refresh_token")
              .HasMaxLength(512);

        entity.Property(u => u.RefreshTokenExpiryTime)
              .HasColumnName("refresh_token_expiry_time");

        entity.Property(u => u.CreatedAt)
              .HasColumnName("created_at")
              .IsRequired();
        entity.Property(u => u.UpdatedAt)
              .HasColumnName("updated_at")
              .IsRequired();
      });
      
      modelBuilder.Entity<ConfirmationToken>(entity=>
      {
        entity.ToTable("confirmation_tokens");
        entity.HasKey(ct => ct.Id);

        entity.Property(ct => ct.UserId)
              .IsRequired();

        entity.Property(ct => ct.Token)
              .IsRequired()
              .HasMaxLength(256);

        entity.Property(ct => ct.Type)
              .IsRequired()
              .HasConversion<string>()
              .HasMaxLength(50);

        entity.Property(ct => ct.ExpirationAt)
              .HasColumnName("expires_at")
              .IsRequired();

        entity.Property(ct => ct.CreatedAt)
              .HasColumnName("created_at")
              .IsRequired();
        
        entity.HasOne<User>()
              .WithMany(u => u.ConfirmationTokens)
              .HasForeignKey(ct => ct.UserId)
              .OnDelete(DeleteBehavior.Cascade);
      });

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

      modelBuilder.Entity<Subscription>(entity =>
      {
          entity.ToTable("Subscription");
          entity.HasKey(s => s.Id);

          entity.Property(s => s.UserId)
                .IsRequired();

          entity.Property(s => s.PackageId)
                .IsRequired();

          entity.Property(s => s.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

          entity.Property(s => s.StartsAt)
                .IsRequired();

          entity.Property(s => s.EndsAt)
                .IsRequired();

          entity.Property(s => s.CreatedAt)
                .IsRequired();

          entity.Property(s => s.UpdatedAt)
                .IsRequired();

          entity.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

          entity.HasOne(s => s.Package)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<SearchLog>(entity =>
      {
          entity.ToTable("search_logs");
          entity.HasKey(sl => sl.Id);

          entity.Property(sl => sl.UserId)
                .IsRequired();

          entity.Property(sl => sl.Keyword)
                .IsRequired()
                .HasMaxLength(255);

          entity.Property(sl => sl.ResultCount)
                .IsRequired();

          entity.Property(sl => sl.Timestamp)
                .IsRequired();

          entity.HasOne(sl => sl.User)
                .WithMany()
                .HasForeignKey(sl => sl.UserId)
                .OnDelete(DeleteBehavior.Cascade);
      });

      modelBuilder.Entity<StatsReport>(entity =>
      {
          entity.ToTable("stats_reports");
          entity.HasKey(sr => sr.Id);

          entity.Property(sr => sr.UserId)
                .IsRequired();

          entity.Property(sr => sr.ActionType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

          entity.Property(sr => sr.Timestamp)
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
    }
  }
}
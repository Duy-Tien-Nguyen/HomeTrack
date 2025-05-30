namespace HomeTrack.Api.Request
{
  public class PackageDto
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } // Đổi tên từ isActive cho C# convention
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
  }

  public class CreatePackageDto
  {
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true; // Mặc định là active khi tạo mới
  }

  public class UpdatePackageDto
  {
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
  }

  public class DeletePackageDto
  {
    public required string packageId{ get; set; }
  }
}
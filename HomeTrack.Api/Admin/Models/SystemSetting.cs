using System.ComponentModel;
using HomeTrack.Domain;

public class SystemSetting
{
  public int Id { get; set; }
  public string SettingKey { get; set; } = string.Empty;
  public int SettingValue { get; set; }
  public DateTime updateAt { get; set; }

  public SystemSetting()
  {
    updateAt = DateTime.UtcNow;
  }
}
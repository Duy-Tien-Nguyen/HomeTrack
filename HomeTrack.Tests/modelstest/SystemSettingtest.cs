using System;
using Xunit;
using HomeTrack.Domain;

public class SystemSettingTests
{
    [Fact]
    public void Constructor_Should_Set_UpdateAt_To_CurrentUtcTime()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var setting = new SystemSetting();

        // Assert
        var after = DateTime.UtcNow;
        Assert.InRange(setting.updateAt, before, after);
    }

    [Fact]
    public void Properties_Should_Set_And_Get_Correctly()
    {
        // Arrange
        var setting = new SystemSetting
        {
            Id = 1,
            SettingKey = "MaxItems",
            SettingValue = 100,
            updateAt = new DateTime(2025, 6, 12)
        };

        // Act & Assert
        Assert.Equal(1, setting.Id);
        Assert.Equal("MaxItems", setting.SettingKey);
        Assert.Equal(100, setting.SettingValue);
        Assert.Equal(new DateTime(2025, 6, 12), setting.updateAt);
    }
}

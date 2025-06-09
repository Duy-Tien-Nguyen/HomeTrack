using HomeTrack.Api.Request;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using Microsoft.AspNetCore.Authorization;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/packages")]
  [Authorize(Roles = "Admin")]
  public class PackageController : ControllerBase
  {
    private readonly IPackageService _packageService;

    public PackageController(IPackageService packageService)
    {
      _packageService = packageService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPackageById(int id)
    {
      try
      {
        var package = await _packageService.GetByIdAsync(id);
        if (package == null)
          return NotFound("Không tìm thấy gói.");
        return Ok(package);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy gói: {ex.Message}");
      }
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllPackages()
    {
      try
      {
        var packages = await _packageService.GetAllAsync();
        if (packages == null || !packages.Any())
          return NotFound("Không có gói nào được tìm thấy.");
        return Ok(packages);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy danh sách gói: {ex.Message}");
      }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto packageDto)
    {
      try
      {
        if (packageDto == null)
          return BadRequest("Gói dữ liệu là bắt buộc.");

        var package = await _packageService.AddAsync(packageDto);
        if (package == null)
          return StatusCode(500, "Lỗi xảy ra khi tạo gói mới.");

        return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi tạo gói: {ex.Message}");
      }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageDto packageDto)
    {
      try
      {
        if (packageDto == null)
          return BadRequest("Gói dữ liệu là bắt buộc.");

        var updated = await _packageService.UpdateAsync(id, packageDto);
        if (!updated)
          return NotFound("Gói không tồn tại hoặc không thể cập nhật.");

        return Ok("Gói đã được cập nhật thành công.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi cập nhật gói: {ex.Message}");
      }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeletePackage(int id)
    {
      try
      {
        var deleted = await _packageService.DeleteAsync(id);
        if (!deleted)
          return NotFound("Gói không tồn tại hoặc không thể xóa.");

        return NoContent();
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi xóa gói: {ex.Message}");
      }
    }

    [HttpPost("toggle-status")]
    public async Task<IActionResult> TogglePackageStatus(int id)
    {
      try
      {
        var toggled = await _packageService.TogglePackageStatusAsync(id);
        if (!toggled)
          return NotFound("Gói không tồn tại hoặc không thể thay đổi trạng thái.");

        return Ok("Trạng thái gói đã được thay đổi thành công.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi thay đổi trạng thái gói: {ex.Message}");
      }
    }
  }
}

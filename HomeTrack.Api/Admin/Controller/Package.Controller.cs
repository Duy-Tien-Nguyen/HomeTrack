using HomeTrack.Domain;
using HomeTrack.Api.Request;
using HomeTrack.Application.Services;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/packages")]
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
      var package = await _packageService.GetByIdAsync(id);
      if (package == null)
      {
        return NotFound();
      }
      return Ok(package);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllPackages()
    {
      var packages = await _packageService.GetAllAsync();
      if (packages == null || !packages.Any())
      {
        return NotFound();
      }
      return Ok(packages);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDto packageDto)
    {
      if (packageDto == null)
      {
        return BadRequest("Package data is required.");
      }

      var package = await _packageService.AddAsync(packageDto);
      if (package == null)
      {
        return StatusCode(500, "An error occurred while creating the package.");
      }
      return CreatedAtAction(nameof(GetPackageById), new { id = package.Id }, package);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageDto packageDto)
    {
      if (packageDto == null)
      {
        return BadRequest("Package data is required.");
      }

      var updated = await _packageService.UpdateAsync(id, packageDto);
      if (!updated)
      {
        return NotFound();
      }
      return NoContent();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeletePackage(int id)
    {
      var deleted = await _packageService.DeleteAsync(id);
      if (!deleted)
      {
        return NotFound();
      }
      return NoContent();
    }

    [HttpPost("toggle-status")]
    public async Task<IActionResult> TogglePackageStatus(int id)
    {
      var toggled = await _packageService.TogglePackageStatusAsync(id);
      if (!toggled)
      {
        return NotFound();
      }
      return NoContent();
    }
  }
}
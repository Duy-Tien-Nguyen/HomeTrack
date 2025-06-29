using HomeTrack.Domain;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore; 
using HomeTrack.Api.Request;


namespace HomeTrack.Application.Services
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDBContext _context;

        public LocationService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<LocationResponseDto>> CreateLocationAsync(LocationCreateRequestDto locationDto, int userId)
        {
            try
            {
                // Kiểm tra tính duy nhất của tên Location cho người dùng (FR3.1)
                bool nameExists = await _context.Locations
                                                .AnyAsync(l => l.UserId == userId &&
                                                               l.Name.ToLower() == locationDto.Name.ToLower());
                if (nameExists)
                {
                    return ServiceResult<LocationResponseDto>.Failure("Tên vị trí đã tồn tại cho người dùng này.");
                }

                // Kiểm tra ParentLocationId nếu có
                if (locationDto.ParentLocationId.HasValue)
                {
                    var parentLocation = await _context.Locations
                                                       .FirstOrDefaultAsync(l => l.Id == locationDto.ParentLocationId.Value &&
                                                                                 l.UserId == userId);
                    if (parentLocation == null)
                    {
                        return ServiceResult<LocationResponseDto>.Failure("Vị trí cha không hợp lệ hoặc không thuộc về bạn.");
                    }
                }

                var newLocation = new Location
                {
                    Name = locationDto.Name,
                    Description = locationDto.Description,
                    ParentLocationId = locationDto.ParentLocationId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    User = null! 
                };

                _context.Locations.Add(newLocation);
                await _context.SaveChangesAsync(); 

                var responseDto = new LocationResponseDto
                {
                    Id = newLocation.Id,
                    Name = newLocation.Name,
                    Description = newLocation.Description,
                    ParentLocationId = newLocation.ParentLocationId,
                    CreatedAt = newLocation.CreatedAt,
                    UpdatedAt = newLocation.UpdatedAt
                };

                return ServiceResult<LocationResponseDto>.Success(responseDto);
            }
            catch (DbUpdateException dbEx)
            {
                return ServiceResult<LocationResponseDto>.Failure($"Lỗi khi lưu vào cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<LocationResponseDto>.Failure($"Đã có lỗi xảy ra trong quá trình tạo vị trí: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<LocationResponseDto>>> GetLocationsAsync(int userId)
        {
            try
            {
                var locations = await _context.Locations
                                              .Where(l => l.UserId == userId)
                                              .OrderBy(l => l.Name) // Sắp xếp theo tên
                                              .Include(l => l.Items)
                                                  .ThenInclude(i => i.ItemTags)
                                                      .ThenInclude(it => it.Tag) // Bao gồm cả Tags của Item
                                              .Select(l => new LocationResponseDto
                                              {
                                                  Id = l.Id,
                                                  Name = l.Name,
                                                  Description = l.Description,
                                                  ParentLocationId = l.ParentLocationId,
                                                  CreatedAt = l.CreatedAt,
                                                  UpdatedAt = l.UpdatedAt,
                                                  Items = l.Items.Where(item => item.DeletedAt == null) // Chỉ lấy các item chưa bị xóa mềm
                                                               .Select(item => new ItemViewModel
                                                               {
                                                                   Id = item.Id,
                                                                   Name = item.Name,
                                                                   Description = item.Description,
                                                                   ImageUrl = item.ImageUrl,
                                                                   LocationId = item.LocationId,
                                                                   Tags = item.ItemTags != null ? item.ItemTags.Select(it => it.Tag.Name).ToList() : new List<string>(),
                                                                   CreatedAt = item.CreatedAt,
                                                                   Color = item.Color
                                                               }).ToList()
                                              })
                                              .ToListAsync();

                return ServiceResult<IEnumerable<LocationResponseDto>>.Success(locations);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<LocationResponseDto>>.Failure($"Đã có lỗi xảy ra khi lấy danh sách vị trí: {ex.Message}");
            }
        }

        public async Task<ServiceResult<LocationResponseDto>> GetLocationByIdAsync(int locationId, int userId)
        {
            try
            {
                var location = await _context.Locations
                                             .FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);

                if (location == null)
                {
                    return ServiceResult<LocationResponseDto>.Failure("Không tìm thấy vị trí hoặc bạn không có quyền truy cập.");
                }

                var responseDto = new LocationResponseDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Description = location.Description,
                    ParentLocationId = location.ParentLocationId,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                };

                return ServiceResult<LocationResponseDto>.Success(responseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<LocationResponseDto>.Failure($"Đã có lỗi xảy ra khi lấy thông tin vị trí: {ex.Message}");
            }
        }

        public async Task<ServiceResult<LocationResponseDto>> UpdateLocationAsync(int locationId, LocationUpdateRequestDto locationDto, int userId)
        {
            try
            {
                // Tìm vị trí cần cập nhật và xác thực quyền sở hữu
                var existingLocation = await _context.Locations
                                                     .FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);

                if (existingLocation == null)
                {
                    return ServiceResult<LocationResponseDto>.Failure("Không tìm thấy vị trí hoặc bạn không có quyền chỉnh sửa.");
                }

                // Cập nhật các thuộc tính nếu có thay đổi
                if (locationDto.Name != null)
                {
                    // Kiểm tra tính duy nhất của tên Location cho người dùng (nếu tên thay đổi)
                    if (locationDto.Name.ToLower() != existingLocation.Name.ToLower())
                    {
                        bool nameExists = await _context.Locations
                                                        .AnyAsync(l => l.UserId == userId &&
                                                                        l.Name.ToLower() == locationDto.Name.ToLower() &&
                                                                        l.Id != locationId); // Loại trừ chính nó

                        if (nameExists)
                        {
                            return ServiceResult<LocationResponseDto>.Failure("Tên vị trí đã tồn tại cho người dùng này.");
                        }
                    }
                    existingLocation.Name = locationDto.Name;
                }

                if (locationDto.Description != null)
                {
                    existingLocation.Description = locationDto.Description;
                }

                if (locationDto.ParentLocationId.HasValue)
                {
                    // Kiểm tra ParentLocationId mới có hợp lệ không (không tự trỏ đến chính nó, không trỏ đến con của nó)
                    if (locationDto.ParentLocationId.Value == locationId)
                    {
                        return ServiceResult<LocationResponseDto>.Failure("Vị trí cha không thể là chính nó.");
                    }
                    // TODO: Cần thêm logic để kiểm tra không trỏ đến con của nó (để tránh vòng lặp)
                    var newParentLocation = await _context.Locations
                                                          .FirstOrDefaultAsync(l => l.Id == locationDto.ParentLocationId.Value &&
                                                                                    l.UserId == userId);
                    if (newParentLocation == null)
                    {
                        return ServiceResult<LocationResponseDto>.Failure("Vị trí cha mới không hợp lệ hoặc không thuộc về bạn.");
                    }
                    existingLocation.ParentLocationId = locationDto.ParentLocationId.Value;
                }
                else if (locationDto.ParentLocationId == null && existingLocation.ParentLocationId.HasValue)
                {
                    // Nếu muốn xóa ParentLocationId (ví dụ biến một kệ thành phòng)
                    existingLocation.ParentLocationId = null;
                }


                existingLocation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var responseDto = new LocationResponseDto
                {
                    Id = existingLocation.Id,
                    Name = existingLocation.Name,
                    Description = existingLocation.Description,
                    ParentLocationId = existingLocation.ParentLocationId,
                    CreatedAt = existingLocation.CreatedAt,
                    UpdatedAt = existingLocation.UpdatedAt
                };

                return ServiceResult<LocationResponseDto>.Success(responseDto);
            }
            catch (DbUpdateException dbEx)
            {
                return ServiceResult<LocationResponseDto>.Failure($"Lỗi khi lưu vào cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<LocationResponseDto>.Failure($"Đã có lỗi xảy ra trong quá trình cập nhật vị trí: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteLocationAsync(int locationId, int userId)
        {
            try
            {
                // Tìm vị trí cần xóa và xác thực quyền sở hữu
                var locationToDelete = await _context.Locations
                                                     .Include(l => l.Items) 
                                                     .Include(l => l.ChildLocations) 
                                                     .FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);

                if (locationToDelete == null)
                {
                    return ServiceResult<bool>.Failure("Không tìm thấy vị trí hoặc bạn không có quyền xóa.");
                }

                // Kiểm tra nếu có bất kỳ Item hoặc ChildLocation nào đang liên kết với Location này
                if (locationToDelete.Items != null && locationToDelete.Items.Any())
                {
                    return ServiceResult<bool>.Failure("Không thể xóa vị trí này vì nó đang chứa các đồ vật.");
                }

                if (locationToDelete.ChildLocations != null && locationToDelete.ChildLocations.Any())
                {
                    return ServiceResult<bool>.Failure("Không thể xóa vị trí này vì nó đang chứa các vị trí con. Vui lòng xóa các vị trí con trước.");
                }

                // Xóa Location khỏi database
                _context.Locations.Remove(locationToDelete);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                return ServiceResult<bool>.Failure($"Lỗi khi xóa vị trí trong cơ sở dữ liệu: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Đã có lỗi xảy ra trong quá trình xóa vị trí: {ex.Message}");
            }
        }
    }
}

using HomeTrack.Api.Request;
using HomeTrack.Domain;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; 
using HomeTrack.Domain.Enum;

namespace HomeTrack.Application.Services
{
public class ItemService : IItemService
{
    private readonly ApplicationDBContext _context; 
    private readonly IWebHostEnvironment _hostEnvironment; 
    public ItemService(ApplicationDBContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment; 
    }
    public async Task<ServiceResult<ItemViewModel>> CreateNewItem(CreateItemDto itemDto, int userId)
    {
        try
        {
            string? imageUrl = null;
            if (itemDto.ImageFile != null && itemDto.ImageFile.Length > 0)
            {
                if (itemDto.ImageFile.Length > 5 * 1024 * 1024) // Kích thước file ảnh tối đa 5MB
                {
                    return ServiceResult<ItemViewModel>.Failure("Kích thước file ảnh không được vượt quá 5MB.");
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(itemDto.ImageFile.FileName);
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath ?? "wwwroot", "uploads", "items");

                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await itemDto.ImageFile.CopyToAsync(stream); 
                }

                imageUrl = $"/uploads/items/{uniqueFileName}";
            }

            var newItemEntity = new Item 
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                ImageUrl = imageUrl, 
                UserId = userId,
                User = null!, 
                LocationId = itemDto.LocationId, 
                Color = itemDto.Color,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null 
            };

            _context.Items.Add(newItemEntity);
            await _context.SaveChangesAsync(); 

            var tagNamesForViewModel = new List<string>();
            if (itemDto.Tags != null && itemDto.Tags.Any())
            {
                foreach (var tagName in itemDto.Tags.Distinct(StringComparer.OrdinalIgnoreCase)) 
                {
                    if (string.IsNullOrWhiteSpace(tagName)) continue;

                    var normalizedTagName = tagName.Trim().ToLower(); 
                    var tagEntity = await _context.Tags
                                    .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTagName);

                    if (tagEntity == null) 
                    {
                        tagEntity = new Tag { Name = tagName.Trim(), CreatedAt = DateTime.UtcNow, User = null! }; 
                        _context.Tags.Add(tagEntity);
                        await _context.SaveChangesAsync(); 
                    }
                    tagNamesForViewModel.Add(tagEntity.Name);

                    var newItemTag = new ItemTag
                    {
                        ItemId = newItemEntity.Id, 
                        TagId = tagEntity.Id,
                        Item = null!, 
                        Tag = null! 
                    };
                    _context.ItemTags.Add(newItemTag);
                }

                await _context.SaveChangesAsync(); 
            }

            // Ghi log hành động "created" vào StatsReport (UC09)
            _context.StatsReports.Add(new StatsReport
            {
                UserId = userId,
                ItemId = newItemEntity.Id,
                ActionType = ActionType.Created,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            var itemViewModel = new ItemViewModel
            {
                Id = newItemEntity.Id,
                Name = newItemEntity.Name,
                Description = newItemEntity.Description,
                ImageUrl = newItemEntity.ImageUrl,
                LocationId = newItemEntity.LocationId,
                Tags = tagNamesForViewModel,
                CreatedAt = newItemEntity.CreatedAt,
                Color = newItemEntity.Color
            };

            return ServiceResult<ItemViewModel>.Success(itemViewModel);
        }
        catch (DbUpdateException dbEx) 
        {
            return ServiceResult<ItemViewModel>.Failure($"Lỗi khi lưu vào cơ sở dữ liệu: {dbEx.Message}");
        }
        catch (IOException ioEx) 
        {
            return ServiceResult<ItemViewModel>.Failure($"Lỗi khi xử lý file ảnh: {ioEx.Message}");
        }
        catch (Exception ex) 
        {
            return ServiceResult<ItemViewModel>.Failure($"Đã có lỗi xảy ra trong quá trình xử lý: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ItemViewModel>> GetItemByIdAsync(int itemId, int userId)
    {
        try
        {
            var item = await _context.Items
                                     .Include(i => i.Location) 
                                     .Include(i => i.ItemTags) 
                                         .ThenInclude(it => it.Tag) 
                                     .FirstOrDefaultAsync(i => i.Id == itemId &&
                                                               i.UserId == userId &&
                                                               i.DeletedAt == null);

            if (item == null)
            {
                return ServiceResult<ItemViewModel>.Failure("Không tìm thấy đồ vật hoặc bạn không có quyền truy cập.");
            }

            var tags = item.ItemTags?.Select(it => it.Tag.Name).ToList();

            var itemViewModel = new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                LocationId = item.LocationId,
                Tags = tags,
                CreatedAt = item.CreatedAt,
                Color = item.Color,
                Location = item.Location != null ? new LocationResponseDto
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Description = item.Location.Description,
                    ParentLocationId = item.Location.ParentLocationId,
                    CreatedAt = item.Location.CreatedAt,
                    UpdatedAt = item.Location.UpdatedAt,
                    Items = null
                } : null
            };

            return ServiceResult<ItemViewModel>.Success(itemViewModel);
        }
        catch (Exception ex)
        {
            return ServiceResult<ItemViewModel>.Failure($"Đã có lỗi xảy ra trong quá trình lấy thông tin đồ vật: {ex.Message}");
        }
    }

    public async Task<ServiceResult<ItemViewModel>> UpdateItemAsync(int itemId, ItemUpdateRequestDto itemDto, int userId)
    {
        try
        {
            var existingItem = await _context.Items
                                             .Include(i => i.ItemTags)
                                                 .ThenInclude(it => it.Tag)
                                             .FirstOrDefaultAsync(i => i.Id == itemId &&
                                                                       i.UserId == userId &&
                                                                       i.DeletedAt == null);

            if (existingItem == null)
            {
                return ServiceResult<ItemViewModel>.Failure("Không tìm thấy đồ vật hoặc bạn không có quyền chỉnh sửa.");
            }

            bool locationChanged = false; // Biến cờ để kiểm tra LocationId có thay đổi không

            if (itemDto.Name != null)
            {
                existingItem.Name = itemDto.Name;
            }
            if (itemDto.Description != null)
            {
                existingItem.Description = itemDto.Description;
            }
            if (itemDto.Color != null)
            {
                existingItem.Color = itemDto.Color;
            }

            if (itemDto.LocationId.HasValue && itemDto.LocationId != existingItem.LocationId)
            {
                var newLocation = await _context.Locations.FirstOrDefaultAsync(l => l.Id == itemDto.LocationId.Value && l.UserId == userId);
                if (newLocation == null)
                {
                    return ServiceResult<ItemViewModel>.Failure("Vị trí mới không hợp lệ hoặc không thuộc về bạn.");
                }

                existingItem.LocationId = itemDto.LocationId.Value;
                locationChanged = true; // Đánh dấu là LocationId đã thay đổi
            }
            else if (itemDto.LocationId.HasValue == false && existingItem.LocationId.HasValue)
            {
                existingItem.LocationId = null;
                locationChanged = true; // Đánh dấu là LocationId đã thay đổi
            }


            if (itemDto.ImageFile != null && itemDto.ImageFile.Length > 0)
            {
                if (itemDto.ImageFile.Length > 5 * 1024 * 1024) // Kích thước file ảnh tối đa 5MB
                {
                    return ServiceResult<ItemViewModel>.Failure("Kích thước file ảnh không được vượt quá 5MB.");
                }

                if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_hostEnvironment.WebRootPath ?? "wwwroot", existingItem.ImageUrl.TrimStart('/'));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(itemDto.ImageFile.FileName);
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath ?? "wwwroot", "uploads", "items");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }
                var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await itemDto.ImageFile.CopyToAsync(stream);
                }
                existingItem.ImageUrl = $"/uploads/items/{uniqueFileName}";
            }

            if (itemDto.Tags != null) 
            {
                _context.ItemTags.RemoveRange(existingItem.ItemTags);
                existingItem.ItemTags.Clear(); 

                foreach (var tagName in itemDto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(tagName)) continue;

                    var normalizedTagName = tagName.Trim().ToLower();
                    var tagEntity = await _context.Tags
                                    .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTagName);

                    if (tagEntity == null)
                    {
                        tagEntity = new Tag { Name = tagName.Trim(), CreatedAt = DateTime.UtcNow, User = null! }; 
                        _context.Tags.Add(tagEntity);
                        await _context.SaveChangesAsync(); 
                    }

                    var newItemTag = new ItemTag
                    {
                        ItemId = existingItem.Id,
                        TagId = tagEntity.Id,
                        Item = existingItem, 
                        Tag = tagEntity
                    };
                    _context.ItemTags.Add(newItemTag);
                }
            }


            existingItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Ghi log hành động "edited" vào StatsReport (UC09)
            _context.StatsReports.Add(new StatsReport
            {
                UserId = userId,
                ItemId = existingItem.Id,
                ActionType = ActionType.Edited,
                Timestamp = DateTime.UtcNow
            });

            // Ghi log hành động "moved" nếu LocationId thay đổi (UC09)
            if (locationChanged)
            {
                _context.StatsReports.Add(new StatsReport
                {
                    UserId = userId,
                    ItemId = existingItem.Id,
                    ActionType = ActionType.Moved,
                    Timestamp = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();

            var tagsForViewModel = existingItem.ItemTags?.Select(it => it.Tag.Name).ToList();
            var itemViewModel = new ItemViewModel
            {
                Id = existingItem.Id,
                Name = existingItem.Name,
                Description = existingItem.Description,
                ImageUrl = existingItem.ImageUrl,
                LocationId = existingItem.LocationId,
                Tags = tagsForViewModel,
                CreatedAt = existingItem.CreatedAt,
                Color = existingItem.Color,
                Location = existingItem.Location != null ? new LocationResponseDto
                {
                    Id = existingItem.Location.Id,
                    Name = existingItem.Location.Name,
                    Description = existingItem.Location.Description,
                    ParentLocationId = existingItem.Location.ParentLocationId,
                    CreatedAt = existingItem.Location.CreatedAt,
                    UpdatedAt = existingItem.Location.UpdatedAt,
                    Items = null
                } : null
            };


            return ServiceResult<ItemViewModel>.Success(itemViewModel);
        }
        catch (DbUpdateException dbEx)
        {
            return ServiceResult<ItemViewModel>.Failure($"Lỗi khi lưu vào cơ sở dữ liệu: {dbEx.Message}");
        }
        catch (IOException ioEx)
        {
            return ServiceResult<ItemViewModel>.Failure($"Lỗi khi xử lý file ảnh: {ioEx.Message}");
        }
        catch (Exception ex)
        {
            return ServiceResult<ItemViewModel>.Failure($"Đã có lỗi xảy ra trong quá trình xử lý: {ex.Message}");
        }
    }

    public async Task<ServiceResult<bool>> DeleteItemAsync(int itemId, int userId)
    {
        try
        {
            var itemToDelete = await _context.Items
                                             .FirstOrDefaultAsync(i => i.Id == itemId &&
                                                                       i.UserId == userId &&
                                                                       i.DeletedAt == null); 

            if (itemToDelete == null)
            {
                return ServiceResult<bool>.Failure("Không tìm thấy đồ vật hoặc bạn không có quyền xóa.");
            }

            itemToDelete.DeletedAt = DateTime.UtcNow;
            itemToDelete.UpdatedAt = DateTime.UtcNow; 

            await _context.SaveChangesAsync();

            // Ghi log hành động "deleted" vào StatsReport (UC09)
            _context.StatsReports.Add(new StatsReport
            {
                UserId = userId,
                ItemId = itemToDelete.Id,
                ActionType = ActionType.Deleted,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // TODO: FR2.4 - Xử lý xóa ảnh và metadata theo retention policy.
            // File ảnh vật lý có thể được xóa bởi một background job sau này.

            return ServiceResult<bool>.Success(true);
        }
        catch (DbUpdateException dbEx)
        {
            return ServiceResult<bool>.Failure($"Lỗi khi xóa đồ vật trong cơ sở dữ liệu: {dbEx.Message}");
        }
        catch (Exception ex)
        {
            return ServiceResult<bool>.Failure($"Đã có lỗi xảy ra trong quá trình xóa đồ vật: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IEnumerable<ItemViewModel>>> GetItemsByLocationAsync(int locationId, int userId)
    {
        try
        {
            // 1. Xác thực locationId thuộc về userId
            var location = await _context.Locations
                                         .FirstOrDefaultAsync(l => l.Id == locationId && l.UserId == userId);

            if (location == null)
            {
                return ServiceResult<IEnumerable<ItemViewModel>>.Failure("Không tìm thấy vị trí hoặc bạn không có quyền truy cập.");
            }

            // 2. Lấy Items có LocationId khớp và chưa bị soft-delete
            var items = await _context.Items
                                      .Include(i => i.ItemTags)
                                          .ThenInclude(it => it.Tag)
                                      .Where(i => i.LocationId == locationId &&
                                                  i.UserId == userId &&
                                                  i.DeletedAt == null)
                                      .ToListAsync();

            if (!items.Any())
            {
                return ServiceResult<IEnumerable<ItemViewModel>>.Success(new List<ItemViewModel>()); // Trả về danh sách rỗng nếu không có item nào
            }

            // 3. Map entities sang ItemViewModel DTOs
            var itemViewModels = items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                LocationId = item.LocationId,
                Tags = item.ItemTags?.Select(it => it.Tag.Name).ToList(),
                CreatedAt = item.CreatedAt,
                Color = item.Color,
                Location = item.Location != null ? new LocationResponseDto
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Description = item.Location.Description,
                    ParentLocationId = item.Location.ParentLocationId,
                    CreatedAt = item.Location.CreatedAt,
                    UpdatedAt = item.Location.UpdatedAt,
                    Items = null
                } : null
            }).ToList();

            return ServiceResult<IEnumerable<ItemViewModel>>.Success(itemViewModels);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<ItemViewModel>>.Failure($"Đã có lỗi xảy ra khi lấy danh sách đồ vật theo vị trí: {ex.Message}");
        }
    }

    public async Task<ServiceResult<IEnumerable<ItemViewModel>>> GetAllItemsAsync(int userId)
    {
        try
        {
            var items = await _context.Items
                                    .Where(i => i.UserId == userId && i.DeletedAt == null)
                                    .Include(i => i.Location)
                                    .Include(i => i.ItemTags)
                                        .ThenInclude(it => it.Tag)
                                    .ToListAsync();

            var itemViewModels = items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                LocationId = item.LocationId,
                Tags = item.ItemTags?.Select(it => it.Tag.Name).ToList(),
                CreatedAt = item.CreatedAt,
                Color = item.Color,
                Location = item.Location != null ? new LocationResponseDto
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name,
                    Description = item.Location.Description,
                    ParentLocationId = item.Location.ParentLocationId,
                    CreatedAt = item.Location.CreatedAt,
                    UpdatedAt = item.Location.UpdatedAt,
                    Items = null
                } : null
            });

            return ServiceResult<IEnumerable<ItemViewModel>>.Success(itemViewModels);
        }
        catch (Exception ex)
        {
            return ServiceResult<IEnumerable<ItemViewModel>>.Failure($"Đã có lỗi xảy ra trong quá trình lấy tất cả đồ vật: {ex.Message}");
        }
    }
}
}
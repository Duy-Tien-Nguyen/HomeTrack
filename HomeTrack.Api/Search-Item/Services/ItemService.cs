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
        private readonly ISystemSettingService _systemSettingService;
        private readonly ILogger<ItemService> _logger;
        private readonly IGoogleAIStudioModerationService _aiStudioModerationService;
        public ItemService(ApplicationDBContext context, IWebHostEnvironment hostEnvironment,
            IGoogleAIStudioModerationService aiStudioModerationService,
            ISystemSettingService systemSettingService,
            ILogger<ItemService> logger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _aiStudioModerationService = aiStudioModerationService;
            _systemSettingService = systemSettingService;
            _logger = logger;
        }
        public async Task<ServiceResult<ItemViewModel>> CreateNewItem(CreateItemDto itemDto, int userId)
        {
            try
            {
                // ===== KIỂM TRA GIỚI HẠN ITEM THEO ROLE ===
                var currentUser = await _context.Users.FindAsync(userId);
                if (currentUser == null)
                {
                    return ServiceResult<ItemViewModel>.Failure("Người dùng không tồn tại.");
                }

                string itemLimitKey = currentUser.Role == Role.Premium || currentUser.Role == Role.Admin
                                    ? "PremiumPackageItemLimit"
                                    : "BasicPackageItemLimit";
                var itemLimitSetting = await _systemSettingService.GetByKeyAsync(itemLimitKey);
                int maxItemsAllowed = 0;
                if (itemLimitSetting != null )
                {
                    maxItemsAllowed = itemLimitSetting.SettingValue;
                }

                if (maxItemsAllowed > 0) {
                    var currentItemCount = await _context.Items
                                                .CountAsync(i => i.UserId == userId && i.DeletedAt == null);
                    if (currentItemCount >= maxItemsAllowed)
                    {
                        return ServiceResult<ItemViewModel>.Failure($"Bạn đã đạt đến giới hạn số lượng đồ vật cho phép ({maxItemsAllowed}) của gói hiện tại.");
                    }
                }

                // === KẾT THÚC KIỂM TRA GIỚI HẠN ===

                string ? imageUrl = null;
                if (itemDto.ImageFile != null && itemDto.ImageFile.Length > 0)
                {
                    if (itemDto.ImageFile.Length > 5 * 1024 * 1024) // Kích thước file ảnh tối đa 5MB
                    {
                        return ServiceResult<ItemViewModel>.Failure("Kích thước file ảnh không được vượt quá 5MB.");
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(itemDto.ImageFile.FileName);
                    var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath ?? "wwwroot", "uploads", "items", userId.ToString());

                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await itemDto.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = $"/uploads/items/{userId}/{uniqueFileName}";
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
                    ImageModerationStatus = ModerationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    DeletedAt = null
                };

                var associatedTagEntities = new List<Tag>();
                if (itemDto.Tags != null && itemDto.Tags.Any())
                {
                    foreach (var tagName in itemDto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrWhiteSpace(tagName)) continue;
                        var normalizedTagName = tagName.Trim().ToLower();
                        var tagEntity = await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTagName);
                        if (tagEntity == null)
                        {
                            tagEntity = new Tag { Name = tagName.Trim(), CreatedAt = DateTime.UtcNow, UserId = userId, ModerationStatus = TagModerationStatus.Pending }; // Gán UserId cho Tag
                            _context.Tags.Add(tagEntity);
                            // Lưu Tags trước nếu cần ID cho ItemTag ngay, hoặc để SaveChangesAsync xử lý cuối cùng
                            await _context.SaveChangesAsync();
                        }
                        associatedTagEntities.Add(tagEntity);
                    }
                }

                // === TÍCH HỢP AI STUDIO MODERATION ===
                var moderationResult = await _aiStudioModerationService.ModerateItemContentAsync(newItemEntity, associatedTagEntities);

                if (moderationResult.IsPotentiallyHarmful)
                {
                    newItemEntity.ImageModerationStatus = ModerationStatus.Pending; // AI phát hiện, cần Admin duyệt
                    newItemEntity.ModerationNote = $"AI Studio Check: {moderationResult.HarmCategory} - {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}"; // Giới hạn độ dài note
                }
                else if (moderationResult.Feedback != null && moderationResult.Feedback.Contains("API_ERROR") || moderationResult.Feedback != null && moderationResult.Feedback.Contains("EXCEPTION"))
                {
                    newItemEntity.ImageModerationStatus = ModerationStatus.Pending; // Lỗi, cần Admin duyệt
                    newItemEntity.ModerationNote = $"AI Studio Error: {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}";
                }
                else
                {
                    newItemEntity.ImageModerationStatus = ModerationStatus.Approved; // AI Studio cho rằng an toàn
                    newItemEntity.ModeratedAt = DateTime.UtcNow;
                    newItemEntity.ModerationNote = $"Auto-approved by AI Studio. Feedback: {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}";
                }
                // === KẾT THÚC TÍCH HỢP AI STUDIO ===


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
                            tagEntity = new Tag { Name = tagName.Trim(), CreatedAt = DateTime.UtcNow, User = null!, ModerationStatus = (TagModerationStatus)newItemEntity.ImageModerationStatus };
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
                    Color = newItemEntity.Color,
                    ModerationStatus = newItemEntity.ImageModerationStatus, // Trả về trạng thái kiểm duyệt
                    ModerationNote = newItemEntity.ModerationNote
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
                    ModerationStatus = item.ImageModerationStatus,
                    // ModerationNote = item.ModerationNote
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

                bool contentOrTagsChanged = false;

                bool imageChanged = false; // Cờ riêng cho việc thay đổi ảnh

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
                    existingItem.ImageUrl = $"/uploads/items/{userId}/{uniqueFileName}";
                    imageChanged = true; // Đánh dấu ảnh đã thay đổi
                }

                var associatedTagEntitiesForUpdate = new List<Tag>();
                var currentTagNames = existingItem.ItemTags.Select(it => it.Tag.Name.Trim().ToLower()).ToHashSet();
                var newTagNames = itemDto.Tags?.Select(t => t.Trim().ToLower()).Distinct(StringComparer.OrdinalIgnoreCase).ToHashSet() ?? new HashSet<string>();

                if (itemDto.Tags != null && !currentTagNames.SetEquals(newTagNames))
                {
                    _context.ItemTags.RemoveRange(existingItem.ItemTags); // Xóa các tag cũ
                    // existingItem.ItemTags.Clear();

                    foreach (var tagName in itemDto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrWhiteSpace(tagName)) continue;

                        var normalizedTagName = tagName.Trim().ToLower();
                        var tagEntity = await _context.Tags
                                        .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedTagName);

                        if (tagEntity == null)
                        {
                            tagEntity = new Tag { Name = tagName.Trim(), CreatedAt = DateTime.UtcNow, User = null!, ModerationStatus = TagModerationStatus.Pending};
                            _context.Tags.Add(tagEntity);
                            await _context.SaveChangesAsync();
                        }
                        associatedTagEntitiesForUpdate.Add(tagEntity);

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
                
                // === TÍCH HỢP AI STUDIO MODERATION KHI CẬP NHẬT ===
                if (contentOrTagsChanged || imageChanged)
                {
                    // Lấy danh sách Tag entities đã được cập nhật/liên kết với item
                    var updatedAssociatedTags = new List<Tag>();
                    if (itemDto.Tags != null) // Nếu có tag mới được gửi lên
                    {
                        foreach (var tagName in itemDto.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrWhiteSpace(tagName)) continue;
                            var tagEntity = _context.Tags.Local.FirstOrDefault(t => t.Name.Trim().ToLower() == tagName.Trim().ToLower()) // Kiểm tra local trước
                                           ?? await _context.Tags.FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == tagName.Trim().ToLower());
                            if (tagEntity != null) updatedAssociatedTags.Add(tagEntity);
                        }
                    }
                    else // Nếu không có tag mới, dùng tag cũ đã load
                    {
                        updatedAssociatedTags = existingItem.ItemTags.Select(it => it.Tag).ToList();
                    }


                    var moderationResult = await _aiStudioModerationService.ModerateItemContentAsync(existingItem, updatedAssociatedTags);
                    // Quyết định cập nhật trạng thái kiểm duyệt
                    // Ví dụ: Nếu AI báo OK và trạng thái cũ không phải là Approved bởi Admin, thì có thể tự Approved
                    // Hoặc nếu AI báo vi phạm, luôn đặt là Pending
                    if (moderationResult.IsPotentiallyHarmful)
                    {
                        existingItem.ImageModerationStatus = ModerationStatus.Pending;
                        existingItem.ModerationNote = $"AI Studio Re-Check: {moderationResult.HarmCategory} - {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}";
                    }
                    else if (moderationResult.Feedback != null && moderationResult.Feedback.Contains("API_ERROR") || moderationResult.Feedback != null && moderationResult.Feedback.Contains("EXCEPTION"))
                    {
                        existingItem.ImageModerationStatus = ModerationStatus.Pending; // Lỗi, cần Admin duyệt
                        existingItem.ModerationNote = $"AI Studio Error (Re-check): {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}";
                    }
                    else if (existingItem.ImageModerationStatus != ModerationStatus.Approved || existingItem.ModerationByUser == null) // Chỉ auto-approve nếu trước đó không phải do Admin duyệt hoặc đang Pending/Rejected
                    {
                        existingItem.ImageModerationStatus = ModerationStatus.Approved;
                        existingItem.ModeratedAt = DateTime.UtcNow;
                        existingItem.ModerationNote = $"Auto-re-approved by AI Studio after update. Feedback: {moderationResult.Feedback?.Substring(0, Math.Min(moderationResult.Feedback.Length, 500))}";
                    }
                }
                // === KẾT THÚC TÍCH HỢP AI STUDIO ===

                // === CẬP NHẬT TAG STATUS KHI ITEM ĐƯỢC AI RE-APPROVED ===
                foreach (var tag in associatedTagEntitiesForUpdate)
                {
                    if (tag.ModerationStatus == TagModerationStatus.Pending)
                    {
                        tag.ModerationStatus = TagModerationStatus.Approved;
                        _context.Entry(tag).State = EntityState.Modified;
                    }
                }
                // === KẾT THÚC CẬP NHẬT TAG STATUS ===


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
                    ModerationStatus = existingItem.ImageModerationStatus,
                    // ModerationNote = existingItem.ModerationNote
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
                    ModerationStatus = item.ImageModerationStatus,
                    // ModerationNote = item.ModerationNote
                }).ToList();

                return ServiceResult<IEnumerable<ItemViewModel>>.Success(itemViewModels);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ItemViewModel>>.Failure($"Đã có lỗi xảy ra khi lấy danh sách đồ vật theo vị trí: {ex.Message}");
            }
        }
    }
}
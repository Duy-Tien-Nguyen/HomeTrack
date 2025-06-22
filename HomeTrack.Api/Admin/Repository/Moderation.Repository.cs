using HomeTrack.Api.Request;
using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class ModeratedRepository : IModerationRepository
  {
    private readonly ApplicationDBContext _context;
    public ModeratedRepository(ApplicationDBContext context)
    {
      _context = context;
    }

    public async Task<PagedResultDto<PendingItemDto>> GetPendingItemsAsync(PagedQueryParameters queryParameters)
    {
      var query = _context.Items
                                .Include(i => i.User)
                                .Include(i => i.ItemTags)
                                    .ThenInclude(it => it.Tag)
                                .Where(i => i.ImageModerationStatus == ModerationStatus.Pending && i.DeletedAt == null)
                                .AsNoTracking();
      // Sắp xếp
      if (!string.IsNullOrWhiteSpace(queryParameters.SortBy))
      {
        if (queryParameters.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
        {
          query = queryParameters.SortOrder?.ToLower() == "asc"
              ? query.OrderBy(i => i.CreatedAt)
              : query.OrderByDescending(i => i.CreatedAt);
        }
        else if (queryParameters.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
        {
          query = queryParameters.SortOrder?.ToLower() == "asc"
              ? query.OrderBy(i => i.Name)
              : query.OrderByDescending(i => i.Name);
        }
        else
        {
          query = query.OrderByDescending(i => i.CreatedAt);
        }
      }
      else
      {
        query = query.OrderByDescending(i => i.CreatedAt);
      }

      var totalRecords = await query.CountAsync();
      var itemsData = await query
                      .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                      .Take(queryParameters.PageSize)
                      .Select(item => new PendingItemDto

                      {
                        Id = item.Id,
                        Name = item.Name,
                        ImageUrl = item.ImageUrl,
                        UserId = item.UserId,
                        UserEmail = item.User != null ? item.User.Email : "N/A", // Xử lý User có thể null nếu FK không bắt buộc
                        CreatedAt = item.CreatedAt,
                        ImageModerationStatus = item.ImageModerationStatus,
                        AssociatedTags = item.ItemTags.Select(it => new ItemTagViewModel
                        {
                          TagId = it.TagId,
                          TagName = it.Tag.Name, // Đảm bảo Tag không null
                          TagModerationStatus = it.Tag.ModerationStatus
                        }).ToList()
                      })
              .ToListAsync();

      return new PagedResultDto<PendingItemDto>
      {
        Data = itemsData,
        CurrentPage = queryParameters.PageNumber,
        PageSize = queryParameters.PageSize,
        TotalRecords = totalRecords,
        TotalPages = (int)Math.Ceiling(totalRecords / (double)queryParameters.PageSize)
      };
    }

    public async Task<Item?> GetItemByIdForModerationAsync(int itemId)
    {
      return await _context.Items
                              .Include(i => i.User)
                              .Include(i => i.ModerationByUser) // User đã duyệt item này
                              .Include(i => i.ItemTags)
                                  .ThenInclude(it => it.Tag) // Lấy cả thông tin Tag
                              .FirstOrDefaultAsync(i => i.Id == itemId && i.DeletedAt == null);
    }

    public async Task<bool> UpdateItemAsync(Item itemToUpdate)
    {
      if (itemToUpdate == null)
      {
        throw new ArgumentNullException(nameof(itemToUpdate));
      }

      _context.Items.Update(itemToUpdate);

      try
      {
        return await _context.SaveChangesAsync() > 0;
      }
      catch (DbUpdateConcurrencyException ex)
      {
        Console.Error.WriteLineAsync($"Concurrency error updating Item Id {itemToUpdate.Id}: {ex.Message}");
        throw; // Hoặc return false; tùy theo cách bạn muốn xử lý ở tầng trên
      }
      catch (Exception ex)
      {
        Console.Error.WriteLineAsync($"Generic error updating Item Id {itemToUpdate.Id}: {ex.Message}");
        throw;
      }
    }
  }
}

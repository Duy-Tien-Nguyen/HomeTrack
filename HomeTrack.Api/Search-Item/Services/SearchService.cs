using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Api.Request;

namespace HomeTrack.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDBContext _context;

        public SearchService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IEnumerable<ItemViewModel>>> BasicSearchItemsAsync(string keyword, int userId)
        {
            try
            {
                var normalizedKeyword = keyword.ToLower().Trim();

                var items = await _context.Items
                                      .Include(i => i.ItemTags)
                                          .ThenInclude(it => it.Tag)
                                      .Where(i => i.UserId == userId &&
                                                  i.DeletedAt == null &&
                                                  (i.Name.ToLower().Contains(normalizedKeyword) ||
                                                   i.ItemTags.Any(it => it.Tag.Name.ToLower().Contains(normalizedKeyword))))
                                      .Select(item => new ItemViewModel
                                      {
                                          Id = item.Id,
                                          Name = item.Name,
                                          Description = item.Description,
                                          ImageUrl = item.ImageUrl,
                                          LocationId = item.LocationId,
                                          Tags = item.ItemTags.Select(it => it.Tag.Name).ToList(),
                                          CreatedAt = item.CreatedAt,
                                          Color = item.Color
                                      })
                                      .ToListAsync();

                // Log Search
                var searchLog = new SearchLog
                {
                    UserId = userId,
                    Keyword = keyword,
                    ResultCount = items.Count(),
                    Timestamp = DateTime.UtcNow
                };
                _context.SearchLogs.Add(searchLog);

                // Log "searched" action to StatsReport for each item found (if applicable)
                foreach (var item in items)
                {
                    var statsReport = new StatsReport
                    {
                        UserId = userId,
                        ItemId = item.Id,
                        ActionType = ActionType.Searched, // Using the enum
                        Timestamp = DateTime.UtcNow
                    };
                    _context.StatsReports.Add(statsReport);
                }

                await _context.SaveChangesAsync();

                return ServiceResult<IEnumerable<ItemViewModel>>.Success(items);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ItemViewModel>>.Failure($"Đã có lỗi xảy ra khi thực hiện tìm kiếm: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<ItemViewModel>>> AdvancedSearchItemsAsync(AdvancedSearchRequestDto dto, int userId)
        {
            try
            {
                // Authorization: Check if user has Premium subscription
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || user.Role != HomeTrack.Domain.Enum.Role.Premium) // Use fully qualified enum name
                {
                    return ServiceResult<IEnumerable<ItemViewModel>>.Failure("Tính năng tìm kiếm nâng cao chỉ dành cho người dùng Premium.");
                }

                IQueryable<Item> query = _context.Items
                                                 .Include(i => i.ItemTags)
                                                     .ThenInclude(it => it.Tag)
                                                 .Where(i => i.UserId == userId && i.DeletedAt == null);

                // Filter by Tags
                if (dto.Tags != null && dto.Tags.Any())
                {
                    var normalizedTags = dto.Tags.Select(t => t.ToLower().Trim()).ToList();
                    query = query.Where(i => i.ItemTags.Any(it => normalizedTags.Contains(it.Tag.Name.ToLower())));
                }

                // Filter by Color
                if (!string.IsNullOrWhiteSpace(dto.Color))
                {
                    var normalizedColor = dto.Color.ToLower().Trim();
                    query = query.Where(i => i.Color != null && i.Color.ToLower() == normalizedColor);
                }

                // Sorting
                if (!string.IsNullOrWhiteSpace(dto.SortBy))
                {
                    switch (dto.SortBy.ToLower())
                    {
                        case "mru": // Most Recently Used - by UpdatedAt descending
                            query = query.OrderByDescending(i => i.UpdatedAt);
                            break;
                        case "lru": // Least Recently Used - by UpdatedAt ascending
                            query = query.OrderBy(i => i.UpdatedAt);
                            break;
                        default:
                            // No specific sorting or invalid sort by, maintain current order or default order
                            break;
                    }
                }

                var items = await query.Select(item => new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    ImageUrl = item.ImageUrl,
                    LocationId = item.LocationId,
                    Tags = item.ItemTags.Select(it => it.Tag.Name).ToList(),
                    CreatedAt = item.CreatedAt,
                    Color = item.Color
                }).ToListAsync();

                // Log Search
                var searchLog = new SearchLog
                {
                    UserId = userId,
                    Keyword = $"Advanced Search: Tags={string.Join(",", dto.Tags ?? new List<string>())}, Color={dto.Color}",
                    ResultCount = items.Count(),
                    Timestamp = DateTime.UtcNow
                };
                _context.SearchLogs.Add(searchLog);

                // Log "searched" action to StatsReport for each item found (if applicable)
                foreach (var item in items)
                {
                    var statsReport = new StatsReport
                    {
                        UserId = userId,
                        ItemId = item.Id,
                        ActionType = ActionType.Searched, // Using the enum
                        Timestamp = DateTime.UtcNow
                    };
                    _context.StatsReports.Add(statsReport);
                }

                await _context.SaveChangesAsync();

                return ServiceResult<IEnumerable<ItemViewModel>>.Success(items);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<ItemViewModel>>.Failure($"Đã có lỗi xảy ra khi thực hiện tìm kiếm nâng cao: {ex.Message}");
            }
        }
    }
}
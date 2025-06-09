using HomeTrack.Domain;
namespace HomeTrack.Application.Interface
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task ActivateUserAsync(int id);
        Task SaveChangesAsync();
    }
}

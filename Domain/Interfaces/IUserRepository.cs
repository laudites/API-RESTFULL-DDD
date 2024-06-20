using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAndPasswordAsync(string username);
        Task AddAsync(User user);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task UpdateAsync(User user);
    }
}

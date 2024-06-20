using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HubDbContext _context;

        public UserRepository(HubDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByUsernameAndPasswordAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.USUARIOveChangeUSUARIOsync();
        }
        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.USUARIOveChangeUSUARIOsync();
        }
    }
}

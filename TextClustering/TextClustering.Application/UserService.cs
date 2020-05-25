using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TextClustering.Domain;
using TextClustering.Domain.Entities;

namespace TextClustering.Application
{
    public class UserService : IUserService
    {
        private readonly TextClusteringDbContext _dbContext;

        public UserService(TextClusteringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public ValueTask<User> GetById(int id)
        {
            return _dbContext.Users.FindAsync(id);
        }

        public async Task<User> GetByCredentials(string email, string password)
        {
            if (!await _dbContext.Users.AnyAsync())
                await Register(new User
                {
                    FirstName = "Alice",
                    LastName = "LastName",
                    Email = email,
                    Password = password
                });

            return await _dbContext.Users.SingleOrDefaultAsync(user =>
                user.Email == email && password == user.Password);
        }

        public async Task<User> Register(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }
}
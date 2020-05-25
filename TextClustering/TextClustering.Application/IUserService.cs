using System.Collections.Generic;
using System.Threading.Tasks;
using TextClustering.Domain.Entities;

namespace TextClustering.Application
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAll();

        ValueTask<User> GetById(int id);

        Task<User> GetByCredentials(string email, string password);

        Task<User> Register(User user);
    }
}
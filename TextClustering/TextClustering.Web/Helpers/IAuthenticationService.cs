using System.Threading.Tasks;
using TextClustering.Web.Models;

namespace TextClustering.Web.Helpers
{
    public interface IAuthenticationService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);

        User GetCurrentUser();
    }
}
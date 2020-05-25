using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TextClustering.Web.Helpers;
using TextClustering.Web.Models;

namespace TextClustering.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public TokenController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(AuthenticateRequest model)
        {
            AuthenticateResponse response = await _authenticationService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Email or password is incorrect" });

            return Ok(response);
        }
    }
}
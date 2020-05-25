using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TextClustering.Application;
using TextClustering.Web.Helpers;
using TextClustering.Web.Models;

namespace TextClustering.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly IAuthenticationService _authenticationService;

        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IAuthenticationService authenticationService,
            IMapper mapper)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Domain.Entities.User> dbUsers = await _userService.GetAll();
            IEnumerable<User> users = _mapper.Map<IEnumerable<Domain.Entities.User>, IEnumerable<User>>(dbUsers);
            return Ok(users);
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            User currentUser = _authenticationService.GetCurrentUser();
            return Ok(currentUser);
        }
    }
}
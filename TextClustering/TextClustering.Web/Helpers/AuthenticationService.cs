using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TextClustering.Application;
using TextClustering.Application.Settings;
using TextClustering.Web.Models;

namespace TextClustering.Web.Helpers
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpContextAccessor _accessor;

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        private readonly AppSettings _appSettings;

        public AuthenticationService(
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor accessor,
            IUserService userService,
            IMapper mapper)
        {
            _accessor = accessor;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            Domain.Entities.User user = await _userService.GetByCredentials(model.Email, model.Password);
            if (user == null) return null;

            User userModel = _mapper.Map<Domain.Entities.User, User>(user);

            string token = GenerateJwtToken(userModel);

            return new AuthenticateResponse(userModel, token);
        }

        public User GetCurrentUser()
        {
            var user = (User) _accessor.HttpContext.Items["User"];
            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("id", user.Id.ToString())}),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
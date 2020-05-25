using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TextClustering.Application;
using TextClustering.Application.Settings;
using TextClustering.Domain.Entities;

namespace TextClustering.Web.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IMapper _mapper;

        private readonly AppSettings _appSettings;

        public JwtMiddleware(
            RequestDelegate next, 
            IOptions<AppSettings> appSettings,
            IMapper mapper)
        {
            _next = next;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null) await AttachUserToContext(context, userService, token);

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;
                int userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                User contextItem = await userService.GetById(userId);
                context.Items["User"] = _mapper.Map<User, Models.User>(contextItem);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
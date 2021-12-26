using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SlsApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SlsApi.Services
{
    public class JwtTokenService
    {
        private readonly string issuer;
        private readonly string audience;
        private readonly SymmetricSecurityKey symmetricSecurityKey;
        private readonly JwtSecurityTokenHandler jwtSecurityTokenHandler;

        public JwtTokenService(IConfiguration configuration)
        {
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            issuer = configuration["JWT:Issuer"];
            audience = configuration["JWT:Audience"];
            symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
        }

        private string GenerateToken(List<Claim> claims)
        {
            var token = new JwtSecurityToken
            (
                claims: claims,
                issuer: issuer,
                audience: audience,
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return jwtSecurityTokenHandler.WriteToken(token);
        }

        public string GenerateToken(Guid sid)
        {
            var _claims = new List<Claim>
            {
                new Claim(ApplicationClaims.UserID, sid.ToString())
            };

            return GenerateToken(_claims);
        }

        public string GenerateTokenWithRoles(Guid sid, IEnumerable<string> roles)
        {
            var _claims = new List<Claim>
            {
                new Claim(ApplicationClaims.UserID, sid.ToString())
            };

            // Add the custom roles claims inside the token
            foreach (var role in roles)
                _claims.Add(new Claim(ApplicationClaims.UserRole, role));

            return GenerateToken(_claims);
        }

        public string GenerateTokenWithClaims(Guid sid, IEnumerable<Claim> claims)
        {
            var _claims = new List<Claim>
            {
                new Claim(ApplicationClaims.UserID, sid.ToString())
            };

            // Add the custom claims inside the token
            _claims.AddRange(claims);

            return GenerateToken(_claims);
        }
    }
}

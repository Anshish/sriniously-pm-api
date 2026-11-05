using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Infrastructure.Services
{
    public class TokenService(IConfiguration configuration):ITokenService
    {
        public string GenerateToken(User user)
        {
            var secretKey=configuration["Jwt:Key"]
                ??throw new InvalidOperationException("Jwt Key is missing from configuration");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("org_id", user.OrganisationId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)              
            };

            var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        // 4. Serialize to compact string format: "eyJhbGciOi..."
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}

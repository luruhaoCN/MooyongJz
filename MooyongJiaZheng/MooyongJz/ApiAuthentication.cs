using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MooyongCommon;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MooyongJz
{
    public class ApiAuthentication
    {
        private static TokenParameter _tokenParameter = new TokenParameter();
        public static void Authentication()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            _tokenParameter = config.GetSection("tokenParameter").Get<TokenParameter>();
        }

        //这儿是真正的生成Token代码
        public static string GenUserToken(string username, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenParameter.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(_tokenParameter.Issuer, null, claims, expires: DateTime.UtcNow.AddMinutes(_tokenParameter.AccessExpiration), signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }
    }
}

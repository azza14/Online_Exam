﻿using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Online_Exam.Helpers;
using Online_Exam.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Online_Exam.DBContext;
using Microsoft.Extensions.Configuration;

namespace Online_Exam.Authorization

{
    public class JwtUtils : IJwtUtils
    {

        private readonly AppSettings _appSetting;
        private readonly IConfiguration _configuration;
        public JwtUtils(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSetting = appSettings.Value;
            _configuration = configuration;
        }
        public string CreateToken( User user)
        {

            //var tokenHandeler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes("Logging:Tokens:Key");//_appSetting.Secret);
            //var tokenDescriptor = new SecurityTokenDescriptor()
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, user.Id.ToString()),
            //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //        new Claim("Roles", user.Role.ToString()),
            //    }),
            //    Expires = DateTime.UtcNow.AddDays(7),
            //    SigningCredentials=  new SigningCredentials(
            //                              new SymmetricSecurityKey(key),
            //                              SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandeler.CreateToken(tokenDescriptor);
            //return tokenHandeler.WriteToken(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("userName", user.UserName),
                     new Claim("role", user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Logging:Tokens:Key");//_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
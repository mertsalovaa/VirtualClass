﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using VirtualClass.DAL;
using VirtualClass.DAL.Entities;

namespace VirtualClass.DLL.JWT
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly EFContext context;
        private readonly IConfiguration configuration;
        private readonly UserManager<DbUser> userManager;

        public JWTTokenService(
            EFContext _context,
            IConfiguration _configuration,
            UserManager<DbUser> _userManager)
        {
            configuration = _configuration;
            context = _context;
            userManager = _userManager;
        }

        public string CreateToken(DbUser user)
        {
            var roles = userManager.GetRolesAsync(user).Result;
            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("roles", role));
            }

            var jwtTokenSecretKey = configuration.GetValue<string>("SecretPhrase");
            var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSecretKey));
            var signInCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                    signingCredentials: signInCredentials,
                    claims: claims,
                    expires: DateTime.Now.AddDays(2)
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
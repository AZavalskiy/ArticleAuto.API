﻿using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using TFAuto.TFAuto.DAL.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.CosmosRepository.Extensions;

namespace TFAuto.Domain;

public class JWTService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly JWTSettings _jwtSettings;
    public JWTService(IRepository<User> repositoryUser, IOptions<JWTSettings> jwtSettings)
    {
        _repositoryUser = repositoryUser;
        _jwtSettings = jwtSettings.Value;
    }
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
    public async Task<List<Claim>> GetClaims(bool isRefreshToken, string userId, string email)
    {
        var user = await _repositoryUser.GetAsync(c => c.Id == userId).FirstOrDefaultAsync();
        var claims = new List<Claim>
        {
            new Claim("sub", userId),
            new Claim("email", email),
            new Claim("isRefresh", isRefreshToken.ToString()),
            new Claim("roleId", user.RoleId),
        };
        foreach (var permissionid in user.PermissionIds)
        {
            claims.Add(new Claim("permissionId", permissionid));
        }
        return claims;
    }
    public JwtSecurityToken CreateToken(List<Claim> claims, int lifetime)
    {
        return new JwtSecurityToken(

            issuer: _jwtSettings.ValidIssuer,
            audience: _jwtSettings.ValidAudience,
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(lifetime),
            signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(_jwtSettings.IssuerSigningKey), SecurityAlgorithms.HmacSha256));
    }
    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
    }
    public async Task<Token> GenerateTokenMode(string userId, string email)
    {
        var claims = GetClaims(false, userId, email);
        var accessToken = CreateToken(await claims, _jwtSettings.AccessTokenLifetimeInHours);

        claims = GetClaims(true, userId, email);
        var refreshToken = CreateToken(await claims, _jwtSettings.RefreshTokenLifetimeInHours);

        var now = DateTime.UtcNow;
        var tokenModel = new Token()
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken),
            AccessTokenExpireDate = now.AddHours(_jwtSettings.AccessTokenLifetimeInHours),
            RefreshTokenExpireDate = now.AddHours(_jwtSettings.RefreshTokenLifetimeInHours)
        };
        return tokenModel;
    }
}

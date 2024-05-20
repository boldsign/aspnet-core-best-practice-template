using Grpc.Core;
using DemoMicroService;

namespace DemoMicroService.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DemoSharedLib;
using Microsoft.IdentityModel.Tokens;

public class AuthService : TokenService.TokenServiceBase
{
    public override Task<TokenReply> Token(TokenRequest request, ServerCallContext context)
    {
        // TODO: validate your user credential here, this is hardcoded for demo purpose
        const string issuer = AuthParameters.Issuer;
        const string audience = AuthParameters.Audience;
        var key = Encoding.ASCII.GetBytes(AuthParameters.SecurityKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                    new Claim(JwtRegisteredClaimNames.Email, request.Username),
                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);

        return Task.FromResult(
            new TokenReply()
            {
                Token = stringToken,
            });
    }
}

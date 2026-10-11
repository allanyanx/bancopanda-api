using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BancoPanda.Api.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BancoPanda.Api.Features.Auth;

public class TokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _jwt = options.Value;

    public (string Token, DateTime ExpiraEn) CrearToken(Usuario usuario)
    {
        var expiraEn = DateTime.UtcNow.AddMinutes(_jwt.MinutosExpiracion);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Clave));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Emisor,
            audience: _jwt.Audiencia,
            claims: claims,
            expires: expiraEn,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}

using System.IdentityModel.Tokens.Jwt;

namespace SafeTokenHandler;

public static class JwtSecurityTokenHandlerExtensions
{
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandlerShared = new();
    
    public static string CompileToString(this JwtSecurityToken token) => JwtSecurityTokenHandlerShared.WriteToken(token);

    public static JwtSecurityToken ToJwtSecurityToken(this string token) => JwtSecurityTokenHandlerShared.ReadJwtToken(token);

}
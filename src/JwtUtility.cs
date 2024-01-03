using System.IdentityModel.Tokens.Jwt;

namespace SafeTokenHandler;

public static class JwtUtility
{
    public static string CompileToString(JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static JwtSecurityToken ToJwtSecurityToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }
}
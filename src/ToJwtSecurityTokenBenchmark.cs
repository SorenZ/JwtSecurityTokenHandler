using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BenchmarkDotNet.Attributes;

namespace SafeTokenHandler;

[MemoryDiagnoser]
public class ToJwtSecurityTokenBenchmark
{
    private static readonly JwtSecurityToken JwtSecurityTokenSample = new(
        issuer: "me",
        audience: "you",
        new[] { new Claim("sub", "11228") });
    
    private static readonly string JwtSecurityTokenStringSample = JwtSecurityTokenSample.CompileToString();
    
    
    [Benchmark(Baseline = true)]
    public void JwtInstance()
    {
        var handler = new JwtSecurityTokenHandler();
        handler.ReadJwtToken(JwtSecurityTokenStringSample);
    }
    
    [Benchmark]
    public void JwtStatic()
    {
        JwtSecurityTokenStringSample.ToJwtSecurityToken();
    }
}
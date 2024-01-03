using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BenchmarkDotNet.Attributes;

namespace SafeTokenHandler;

[MemoryDiagnoser]
public class CompileToStringBenchmark
{
    private static readonly JwtSecurityToken JwtSecurityTokenSample = new(
        issuer: "me",
        audience: "you",
        new[] { new Claim("sub", "11228") });
    
    [Benchmark(Baseline = true)]
    public void JwtInstance()
    {
        var handler = new JwtSecurityTokenHandler();
        handler.WriteToken(JwtSecurityTokenSample);
    }
    
    [Benchmark]
    public void JwtStatic()
    {
        JwtSecurityTokenSample.CompileToString();
    }
}
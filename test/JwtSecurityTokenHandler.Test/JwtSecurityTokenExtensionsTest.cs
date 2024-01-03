using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SafeTokenHandler;

namespace JwtSecurityTokenHandler.Test;

public class JwtSecurityTokenExtensionsTest
{
    [Fact]
    public void CompileToString_MultiThread()
    {
        var expected = new Dictionary<int, string>();

        var upper = Random.Shared.Next(1000, 10000);
        
        for (var i = 0; i < upper; i++)
        {
            var token = new JwtSecurityToken(
                issuer: "me",
                audience: "you",
                new[] { new Claim("sub", i.ToString()) });
            
            expected.Add(i, JwtUtility.CompileToString(token));
        }

        var actual = new ConcurrentDictionary<int, string>();   
        
        Parallel.For(0, upper, i =>
        {
            var token = new JwtSecurityToken(
                issuer: "me",
                audience: "you",
                new[] { new Claim("sub", i.ToString()) });

            var addResult = actual.TryAdd(i, token.CompileToString());
            Assert.True(addResult, $"Failed to add {i} to actual");
            
        });
        
        Assert.Equal(expected, actual);
        
        foreach (var (expectedKey, expectedValue) in expected)
        {
            Assert.Equal(expectedValue, actual[expectedKey]);
        }
    }
    
    [Fact]
    public void ToJwtSecurityToken_MultiThread()
    {
        var expected = new Dictionary<int, string>();

        var upper = Random.Shared.Next(1000, 10000);
        
        for (var i = 0; i < upper; i++)
        {
            var token = new JwtSecurityToken(
                issuer: "me",
                audience: "you",
                new[] { new Claim("sub", i.ToString()) });
            
            expected.Add(i, JwtUtility.CompileToString(token));
        }
        
        var actual = new ConcurrentDictionary<int, JwtSecurityToken>();
        
        Parallel.For(0, upper, i =>
        {
            var addResult = actual.TryAdd(i, expected[i].ToJwtSecurityToken());
            Assert.True(addResult, $"Failed to add {i} to actual");
        });
        
        foreach (var (expectedKey, expectedValue) in expected)
        {
            Assert.Equal(expectedValue, actual[expectedKey].CompileToString());
        }
    }
}
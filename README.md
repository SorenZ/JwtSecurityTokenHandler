# Thread Safety Analysis of JwtSecurityTokenHandler Methods
## Overview
This document provides an in-depth analysis of the thread safety for two critical methods in the `JwtSecurityTokenHandler` class: `WriteToken` and `ReadJwtToken`. 
We present benchmark results and a detailed explanation of tests to demonstrate their behavior under concurrent usage.

## The code we are talking about:
```csharp
using System.IdentityModel.Tokens.Jwt;

namespace SafeTokenHandler;

public static class JwtSecurityTokenHandlerExtensions
{
    private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandlerShared = new();
    
    public static string CompileToString(this JwtSecurityToken token) =>
      JwtSecurityTokenHandlerShared.WriteToken(token);

    public static JwtSecurityToken ToJwtSecurityToken(this string token) =>
      JwtSecurityTokenHandlerShared.ReadJwtToken(token);
}
```

## Benchmark Results
### WriteToken
```

BenchmarkDotNet v0.13.11, macOS Sonoma 14.0 (23A344) [Darwin 23.0.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method      | Mean       | Error   | StdDev  | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------ |-----------:|--------:|--------:|------:|-------:|-------:|----------:|------------:|
| JwtInstance | 1,403.7 ns | 5.11 ns | 4.27 ns |  1.00 | 0.9537 | 0.0172 |    7.8 KB |        1.00 |
| JwtStatic   |   479.9 ns | 1.39 ns | 1.16 ns |  0.34 | 0.3195 |      - |   2.62 KB |        0.34 |


### ReadJwtToken
```

BenchmarkDotNet v0.13.11, macOS Sonoma 14.0 (23A344) [Darwin 23.0.0]
Apple M2 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 8.0.100
  [Host]     : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 8.0.0 (8.0.23.53103), Arm64 RyuJIT AdvSIMD


```
| Method      | Mean       | Error   | StdDev  | Ratio | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------ |-----------:|--------:|--------:|------:|-------:|-------:|----------:|------------:|
| JwtInstance | 1,806.3 ns | 7.24 ns | 6.78 ns |  1.00 | 0.8755 | 0.0095 |   7.16 KB |        1.00 |
| JwtStatic   |   869.9 ns | 1.79 ns | 1.40 ns |  0.48 | 0.2403 | 0.0010 |   1.97 KB |        0.28 |


## Test Explanation

### WriteToken Method Testing
#### Test Objective
The primary goal is to evaluate the thread safety and performance efficiency of the `WriteToken` method when used concurrently in high-load scenarios.

#### Test Methodology
The benchmarking performed in `CompileToStringBenchmark.cs` involved testing the `WriteToken` method under a simulated high-concurrency environment. 
JWT tokens were generated and passed to WriteToken in a parallel execution context, mimicking a real-world scenario where multiple threads may request token generation simultaneously.

Code Snippet:
```csharp
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
```
This snippet demonstrates the approach used for benchmarking the `WriteToken` method, highlighting its concurrent execution.

### ReadJwtToken Method Testing
#### Test Objective
To assess the thread safety and performance of the `ReadJwtToken` method, particularly in situations where multiple threads are reading JWT tokens concurrently.

### Test Methodology
The test conducted in `ToJwtSecurityTokenBenchmark.cs` involved parallel processing to read JWT tokens. 
This simulated a high-traffic scenario where numerous threads are simultaneously reading tokens, a common case in distributed systems dealing with authentication and authorization tasks.

Code Snippet:
```csharp
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
```
The above code illustrates the benchmarking of `ReadJwtToken` under concurrent conditions, offering insights into its performance and thread safety.

### Functional Correctness
#### Test Overview
The functional correctness of the `WriteToken` and `ReadJwtToken` methods was tested in JwtSecurityTokenExtensionsTest.cs. 
This test aimed to verify that these methods produce consistent and accurate results when invoked simultaneously by multiple threads.

#### Key Findings
- **Consistency in Results:** The tests confirmed that both methods returned consistent results across multiple threads, ensuring reliability in a concurrent environment.
- **No Data Corruption or Loss:** There was no evidence of data corruption or loss, indicating that these methods handle concurrent requests effectively.

## Conclusion
The conducted benchmarks and functional tests indicate that the `WriteToken` and `ReadJwtToken` methods are thread-safe. 
These results are crucial for applications requiring high concurrency, ensuring reliable and consistent performance under various load conditions.











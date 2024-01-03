// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using SafeTokenHandler;

Console.WriteLine("Hi, Mom!");
BenchmarkRunner.Run<ToJwtSecurityTokenBenchmark>();
BenchmarkRunner.Run<CompileToStringBenchmark>();
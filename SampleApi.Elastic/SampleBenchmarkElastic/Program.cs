// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using SampleBenchmarkElastic;

Console.WriteLine("Benchmark Elastic vs Mysql");

var config = ManualConfig
    .Create(DefaultConfig.Instance)
    .WithOptions(ConfigOptions.DisableOptimizationsValidator)
    .AddLogger(ConsoleLogger.Default);

var summary = BenchmarkRunner.Run<ApiBenchmark>(config);

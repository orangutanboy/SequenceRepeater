using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using SequenceRepeater;

public class Benchmark
{
    public static void Main(string[] args)
    {
        var config = ManualConfig.CreateMinimumViable()
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddExporter(CsvExporter.Default)
            .AddExporter(MarkdownExporter.Default)
            .AddExporter(PlainExporter.Default)
            .WithOptions(ConfigOptions.DontOverwriteResults)
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .AddJob(Job.Default.WithIterationCount(4).WithWarmupCount(2).WithInvocationCount(16));

        BenchmarkRunner.Run<Benchmark>(config);
    }

    [Benchmark(Description = "Span")]
    public void SpanTest()
    {
        var repeatSearcher = new RepeatSearcher();
        RunSimulation(repeatSearcher);
    }

    [Benchmark(Description = "Array")]
    public void ArrayTest()
    {
        var repeatSearcher = new RepeatSearcherArray();
        RunSimulation(repeatSearcher);
    }

    [Benchmark(Description = "Enumerable")]
    public void EnumerableTest()
    {
        var repeatSearcher = new RepeatSearcherEnumerable();
        RunSimulation(repeatSearcher);
    }

    private static void RunSimulation(IRepeatSearcher repeatSearcher)
    {
        for (var repeatLength = 25; repeatLength < 400; repeatLength += 25) // total length of repeat
        {
            var sequence = Enumerable.Range(0, repeatLength).Select(i => i.ToString()).ToList();

            sequence.AddRange(sequence);
            sequence.AddRange(sequence);

            sequence.Add("Extra");

            repeatSearcher.GetLongestRepeated(sequence);
        }
    }
}
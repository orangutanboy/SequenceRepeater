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
        RunTests(repeatSearcher);
    }

    [Benchmark(Description = "Array")]
    public void ArrayTest()
    {
        var repeatSearcher = new RepeatSearcherArray();
        RunTests(repeatSearcher);
    }

    [Benchmark(Description = "Enumerable")]
    public void EnumerableTest()
    {
        var repeatSearcher = new RepeatSearcherEnumerable();
        RunTests(repeatSearcher);
    }

    private void RunTests(IRepeatSearcher repeatSearcher)
    {
        repeatSearcher.GetLongestRepeated(Array.Empty<string>());
        repeatSearcher.GetLongestRepeated(new[] { "a" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "a" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "a" });
        repeatSearcher.GetLongestRepeated(new[] { "b", "a", "a" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "c", "a" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "a", "b" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "a", "b", "a", "b" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "a", "b", "a", "b", "a" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "a", "b", "a", "b", "a", "b" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "c", "a", "b", "c" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "c", "a", "b", "c", "d" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "c", "d", "e", "a", "b", "c", "d", "e" });
        repeatSearcher.GetLongestRepeated(new[] { "a", "b", "c", "d", "e", "f", "a", "b", "c", "d", "e", "f", "a", "b", "c", "d", "e", "f" });

        for (var repeatLength = 25; repeatLength < 400; repeatLength += 25) // total length of repeat
        {
            var sequence = new List<string>();

            for (var i = 0; i < repeatLength; ++i)
            {
                sequence.Add(i.ToString());
            }

            sequence.AddRange(sequence);
            sequence.AddRange(sequence);
            sequence.Add("Extra");

            repeatSearcher.GetLongestRepeated(sequence);
        }
    }
}
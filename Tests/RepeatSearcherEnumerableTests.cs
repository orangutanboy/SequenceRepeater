using SequenceRepeater;

namespace SequenceRepeaterTests
{
    public class RepeatSearcherEnumerableTests
    {
        public static IEnumerable<object[]> TestData()
        {
            return new[]{
                new object[] { Array.Empty<string>(), Array.Empty<string>(), 0 },
                new object[] { new[] { "a" }, Array.Empty<string>(), 0 },
                new object[] { new[] { "a", "b" }, Array.Empty<string>() , 0 },
                new object[] { new[] { "a", "a" }, new[] { "a" }, 2 },
                new object[] { new[] { "a", "b", "a" }, new[] { "a" }, 2 },
                new object[] { new[] { "b", "a", "a" }, new[] { "a" }, 2 },
                new object[] { new[] { "a", "b", "c", "a" }, new[] { "a" }, 2 },
                new object[] { new[] { "a", "b", "a", "b", "a", "b" }, new[] { "a", "b" }, 3 },
                new object[] { new[] { "a", "b", "a", "b", "a", "b", "a" }, new[] { "a", "b" }, 3 },
                new object[] { new[] { "a", "b", "a", "b", "a", "b", "a", "b" }, new[] { "a", "b" }, 4 },
                new object[] { new[] { "a", "b", "a", "b" }, new[] { "a", "b" }, 2 },
                new object[] { new[] { "a", "b", "c", "a", "b", "c" }, new[] { "a", "b", "c" }, 2 },
                new object[] { new[] { "a", "b", "c", "a", "b", "c", "d" }, new[] { "a", "b", "c" }, 2 },
                new object[] { new[] { "a", "b", "c", "d", "e", "a", "b", "c", "d", "e" }, new[] { "a", "b", "c", "d", "e" }, 2 },
                new object[] { new[] { "a", "b", "c", "d", "e", "f", "a", "b", "c", "d", "e", "f", "a", "b", "c", "d", "e", "f" }, new[] { "a", "b", "c", "d", "e", "f" }, 3 },
            };
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TestItAll(string[] sequence, string[] expectedRepeat, int expectedOccurrences)
        {
            var x = new RepeatSearcherEnumerable();
            var actual = x.GetLongestRepeated(sequence);
            Assert.Equal(expectedRepeat, actual.LongestRepeat);
            Assert.Equal(expectedOccurrences, actual.Occurrences);
        }
    }
}

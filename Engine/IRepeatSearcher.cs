namespace SequenceRepeater
{
    public interface IRepeatSearcher
    {
        (IEnumerable<string> LongestRepeat, int Occurrences) GetLongestRepeated(IEnumerable<string> sequence);
    }
}

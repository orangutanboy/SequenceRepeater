namespace SequenceRepeater
{
    public class RepeatSearcherArray : IRepeatSearcher
    {
        public (IEnumerable<string> LongestRepeat, int Occurrences) GetLongestRepeated(IEnumerable<string> sequence)
        {
            var sequenceArray = sequence.ToArray();
            var longestRepeat = GetRepeats(sequenceArray);
            return (longestRepeat, GetCountOfRepeats(sequenceArray, longestRepeat));
        }

        private static string[] GetRepeats(string[] sequence)
        {
            var length = sequence.Length;
            if (length < 2)
            {
                return Array.Empty<string>();
            }

            string[] lastSequence = Array.Empty<string>();

            var repeatLengthCalculator = new RepeatLengthCalculator(length);
            while (repeatLengthCalculator.TryGetNext(out var repeatLength))
            {
                var newSequenceFound = false;
                if (length == repeatLength)
                {
                    return lastSequence == null ? Array.Empty<string>() : lastSequence.ToArray();
                }

                for (var i = 0; i <= length - (repeatLength * 2) && !newSequenceFound; i++)
                {
                    var nextToCheck = sequence[i..(i + repeatLength)];
                    if (!AllDistinct(nextToCheck))
                    {
                        continue;
                    }

                    for (var j = repeatLength; j < length - (repeatLength - 1); j++)
                    {
                        var checkIn = sequence[j..(j + repeatLength)];
                        if (nextToCheck.SequenceEqual(checkIn))
                        {
                            lastSequence = nextToCheck;
                            repeatLengthCalculator.MarkAsSuccess(repeatLength);
                            newSequenceFound = true;
                            break;
                        }
                    }
                }
                if (!newSequenceFound)
                {
                    repeatLengthCalculator.MarkAsFailure(repeatLength);
                }
            }

            return lastSequence.ToArray();
        }

        private static bool AllDistinct(string[] nextToCheck)
        {
            for (var i = 0; i < nextToCheck.Length; ++i)
            {
                for (var j = i + 1; j < nextToCheck.Length; j++)
                {
                    if (nextToCheck[i] == nextToCheck[j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static int GetCountOfRepeats(string[] sequence, string[] repeatingSequence)
        {
            if (sequence.Length == 0 || repeatingSequence.Length == 0)
            {
                return 0;
            }

            var occurences = 0;
            var i = 0;
            while (i <= sequence.Length - repeatingSequence.Length)
            {
                if (sequence[i..(i + repeatingSequence.Length)].SequenceEqual(repeatingSequence))
                {
                    ++occurences;
                }
                i += repeatingSequence.Length;
            }

            return occurences;
        }
    }
}

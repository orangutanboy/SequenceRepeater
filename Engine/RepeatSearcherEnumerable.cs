namespace SequenceRepeater
{
    public class RepeatSearcherEnumerable : IRepeatSearcher
    {
        public (IEnumerable<string> LongestRepeat, int Occurrences) GetLongestRepeated(IEnumerable<string> sequence)
        {
            var longestRepeat = GetRepeats(sequence);
            return (longestRepeat, GetRepeats(sequence, longestRepeat));
        }

        private static IEnumerable<string> GetRepeats(IEnumerable<string> sequence)
        {
            var length = sequence.Count();
            if (length < 2)
            {
                return Enumerable.Empty<string>();
            }

            var lastSequence = Enumerable.Empty<string>();

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
                    var nextToCheck = sequence.Take(i..(i + repeatLength));
                    if (!AllDistinct(nextToCheck))
                    {
                        continue;
                    }

                    for (var j = repeatLength; j < length - (repeatLength - 1); j++)
                    {
                        var checkIn = sequence.Take(j..(j + repeatLength));
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

        private static bool AllDistinct(IEnumerable<string> nextToCheck)
        {
            return nextToCheck.Distinct().Count() == nextToCheck.Count();
        }

        private static int GetRepeats(IEnumerable<string> sequence, IEnumerable<string> repeatingSequence)
        {
            if (!sequence.Any() || !repeatingSequence.Any())
            {
                return 0;
            }

            var occurences = 0;
            var i = 0;
            while (i <= sequence.Count() - repeatingSequence.Count())
            {
                if (sequence.Take(i..(i + repeatingSequence.Count())).SequenceEqual(repeatingSequence))
                {
                    ++occurences;
                }
                i += repeatingSequence.Count();
            }

            return occurences;
        }
    }
}

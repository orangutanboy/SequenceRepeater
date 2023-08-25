namespace SequenceRepeater
{
    public interface IRepeatSearcher
    {
        (IEnumerable<string> LongestRepeat, int Occurrences) GetLongestRepeated(IEnumerable<string> sequence);
    }

    public class RepeatSearcher : IRepeatSearcher
    {
        public (IEnumerable<string> LongestRepeat, int Occurrences) GetLongestRepeated(IEnumerable<string> sequence)
        {
            var sequenceArray = sequence.ToArray();
            var longestRepeat = GetRepeats(sequenceArray);
            return (longestRepeat, GetRepeats(sequenceArray, longestRepeat));
        }

        private static string[] GetRepeats(Span<string> sequence)
        {
            var length = sequence.Length;
            if (length < 2)
            {
                return Array.Empty<string>();
            }

            Span<string> lastSequence = null;

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

        private static bool AllDistinct(Span<string> nextToCheck)
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

        private static int GetRepeats(Span<string> sequence, Span<string> repeatingSequence)
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

    public class RepeatLengthCalculator
    {
        private readonly Dictionary<int, bool> _previousGuesses = new();
        private readonly int _seed;
        private int _last;

        public RepeatLengthCalculator(int seed)
        {
            _last = -1;
            _seed = seed;
        }

        public void MarkAsSuccess(int value)
        {
            _previousGuesses[value] = true;
        }

        public void MarkAsFailure(int value)
        {
            _previousGuesses[value] = false;
        }

        public bool TryGetNext(out int value)
        {
            switch (_last)
            {
                case -1:
                    value = _last = (_seed + 1) / 2;
                    return true;
                case 1:
                    value = 1;
                    return false;
                default:
                    if (_previousGuesses.TryGetValue(_last - 1, out var lastMinusOne) && lastMinusOne)
                    {
                        value = -1;
                        return false;
                    }

                    if (!_previousGuesses[_last])
                    {
                        value = _last = GetNextLowerGuess();
                    }
                    else
                    {
                        value = _last = GetNextHigherGuess();
                    }

                    return !_previousGuesses.TryGetValue(value, out _); // If the new guess has already been used, return false
            }
        }

        private int GetNextLowerGuess()
        {
            var previousLowerGuesses = _previousGuesses.Where(kvp => kvp.Key < _last && kvp.Value);
            if (!previousLowerGuesses.Any())
            {
                return (_last + 1) / 2;
            }
            var nextLowestPreviousGuess = previousLowerGuesses.Select(kvp => kvp.Key).Max();
            var nextGuess = _last - ((_last - nextLowestPreviousGuess) / 2);
            return nextGuess;
        }

        private int GetNextHigherGuess()
        {
            var previousHigherGuesses = _previousGuesses.Where(kvp => kvp.Key > _last);
            if (!previousHigherGuesses.Any())
            {
                return _last + 1;
            }

            var nextHighestPreviousGuess = previousHigherGuesses.Select(kvp => kvp.Key).Min();
            var nextGuess = ((nextHighestPreviousGuess + 1 - _last) / 2) + _last;
            return nextGuess;
        }
    }
}

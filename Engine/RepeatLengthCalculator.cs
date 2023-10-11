namespace SequenceRepeater
{
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

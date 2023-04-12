using System.Diagnostics;

namespace MyBackGroundServices.Controllers
{
    public class LevelSemaphore
    {
        public LevelSemaphore(int initialValue, string? name = "")
        {
            InitialValue = initialValue;
            CurrentValue = initialValue;
            MaxValue = initialValue + 1;
            Name = !string.IsNullOrEmpty(name) ? name : Guid.NewGuid().ToString();
        }

        public LevelSemaphore(int initialValue)
        {
            InitialValue = initialValue;
            CurrentValue = initialValue;
            MaxValue = initialValue + 1;
            Name = Guid.NewGuid().ToString();
        }

        public LevelSemaphore(int initialValue, int maxValue, string? name = "")
        {
            if (maxValue <= initialValue) throw new ArgumentOutOfRangeException(nameof(MaxValue), "Maxvalue should be larger than the initial value");
            InitialValue = initialValue;
            CurrentValue = initialValue;
            MaxValue = maxValue;
            Name = !string.IsNullOrEmpty(name) ? name : Guid.NewGuid().ToString();
        }

        private readonly object _lock = new object();
        public int InitialValue { get; init; }
        public int MaxValue { get; private set; }
        public int CurrentValue { get; private set; }
        public string Name { get; init; }

        public async Task<bool> WaitOne(TimeSpan timeSpan)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                bool success;
                lock (_lock)
                {
                    CurrentValue += 1;
                    if (CurrentValue > MaxValue)
                    {
                        success = false;
                        CurrentValue -= 1;
                    }
                    else
                    {
                        success = true;
                    }
                }
                if (!success)
                {
                    if (stopwatch.Elapsed > timeSpan)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
                await Task.Delay(100);
            }
        }

        public void Release()
        {
            lock (_lock)
            {
                CurrentValue -= 1;
            }
        }
    }
}
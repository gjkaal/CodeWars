using System.Diagnostics;

namespace CoreElements;

/// <summary>
/// The Fork class can be used to start a process in one context
/// and check the completion in another context. The fork class
/// can be used for async class initializers.
/// </summary>
public class Fork
{
    private Exception? ExceptionThrown;
    public bool Completed { get; private set; }
    public Task RunningTask { get; private set; }
    public long TimeOutInMilliSeconds { get; private set; }

    /// <summary>
    /// Initialize with an awaitable task and the timeout for completion.
    /// </summary>
    public Fork(Func<Task> awaitable, long timeOutInMilliSeconds)
    {
        TimeOutInMilliSeconds = timeOutInMilliSeconds;
        RunningTask = Task.Run(async () =>
        {
            try
            {
                await awaitable.Invoke();
                Completed = true;
            }
#pragma warning disable CA1031 // Do not catch generic exceptions (without logging)
            catch (Exception ex)
            {
                ExceptionThrown = ex;
            }
#pragma warning restore CA1031
        });
    }

    /// <summary>
    /// Initialize with a synchronous method and the timeout for completion.
    /// </summary>
    public Fork(Action action, long timeOutInMilliSeconds)
    {
        TimeOutInMilliSeconds = timeOutInMilliSeconds;
        RunningTask = Task.Run(() =>
        {
            try
            {
                action.Invoke();
                Completed = true;
            }
#pragma warning disable CA1031 // Do not catch generic exceptions (without logging)
            catch (Exception ex)
            {
                ExceptionThrown = ex;
            }
#pragma warning restore CA1031
        });
    }

    /// <summary>
    /// Join the current thread to the completion of the forked process.
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    public async Task JoinAsync(CancellationToken cancellation = new CancellationToken())
    {
        if (Completed)
            return;
        if (ExceptionThrown != null)
        {
            throw ExceptionThrown;
        }
        var t = Stopwatch.StartNew();
        while (!Completed)
        {
            await Task.Delay(10, cancellation);
            cancellation.ThrowIfCancellationRequested();
            if (t.ElapsedMilliseconds > TimeOutInMilliSeconds)
            {
                throw new TimeoutException($"Fork process timed out at {t.ElapsedMilliseconds}ms.");
            }
        }
    }
}
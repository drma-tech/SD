using System.Runtime.InteropServices.JavaScript;

namespace SD.WEB.Core;

public static class TaskExtensions
{
    public static void FireAndForget(this Task task)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (task.IsCompletedSuccessfully || task.IsCanceled) return;

        _ = ForgetAwaited(task);
    }

    private static async Task ForgetAwaited(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (JSException ex) when (ex.Message.Contains(nameof(OperationCanceledException), StringComparison.OrdinalIgnoreCase))
        {
            // JS interop can surface cancellation as a JSException.
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}
using TodoApp.Blazor.Services;

namespace TodoApp.Blazor.Utils;

public static class ErrorHandlerExtensions
{
    public static async Task RunSafeAsync(this IErrorHandlerService errorHandler, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            await errorHandler.HandleAsync(ex);
        }
    }
}

namespace TodoApp.Blazor.Services;

public interface IErrorHandlerService
{
    Task HandleAsync(Exception exception);
}

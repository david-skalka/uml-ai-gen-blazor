using System.Net;
using Microsoft.Extensions.Logging;
using MudBlazor;
using TodoApp.Api;
using TodoApp.Blazor.Components.Dialogs;
using TodoApp.Blazor.Utils;

namespace TodoApp.Blazor.Services;

public class ErrorHandlerService(
    IDialogService dialogService,
    ILogger<ErrorHandlerService> logger) : IErrorHandlerService
{
    public async Task HandleAsync(Exception exception)
    {
        if (exception is OperationCanceledException)
            return;

        if (exception is ApiException { StatusCode: (int)HttpStatusCode.BadRequest } apiException)
        {
            logger.LogWarning("Validation error: {Response}", apiException.Response);
            await ShowErrorDialogAsync("Validation error", errors: apiException.GetValidationErrors());
            return;
        }

        logger.LogError(exception, "Unhandled error");
        await ShowErrorDialogAsync(exception.GetType().Name, message: exception.Message);
    }

    private async Task ShowErrorDialogAsync(
        string title,
        string? message = null,
        IReadOnlyList<string>? errors = null)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Message, message ?? string.Empty },
            { x => x.Errors, errors ?? [] },
            {
                x => x.Buttons,
                (IReadOnlyList<ConfirmButton>)[new ConfirmButton("OK", ConfirmResult.Ok, true)]
            }
        };

        var dialog = await dialogService.ShowAsync<ConfirmDialog>(title, parameters, AppDialogOptions.Small);
        await dialog.Result;
    }
}

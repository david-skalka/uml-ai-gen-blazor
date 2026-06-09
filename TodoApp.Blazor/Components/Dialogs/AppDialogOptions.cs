using MudBlazor;

namespace TodoApp.Blazor.Components.Dialogs;

public static class AppDialogOptions
{
    public static DialogOptions Small { get; } = new()
    {
        CloseOnEscapeKey = true,
        BackdropClick = true,
        MaxWidth = MaxWidth.Small,
        FullWidth = true
    };
}

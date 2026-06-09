namespace TodoApp.Blazor.Components.Dialogs;

public enum ConfirmResult
{
    Cancel,
    Ok,
    Delete
}

public sealed record ConfirmButton(string Label, ConfirmResult Result, bool IsPrimary = false);

using Blazor.Sonner.Services;
using Congratulator.WebAssembly.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Congratulator.WebAssembly.Components;

/// <summary>
/// Base component for pages that provides common functionality such as
/// entity deletion with confirmation and clipboard operations.
/// </summary>
public class BasePageComponent : ComponentBase
{
    /// <summary>
    /// Service for displaying toast notifications to the user.
    /// </summary>
    [Inject] 
    public ToastService NotificationService { get; set; } = null!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Executes a delete operation and shows a success or failure notification.
    /// </summary>
    /// <param name="entityName">Display name of the entity being deleted.</param>
    /// <param name="deleteAction">Async function that performs the delete and returns an <see cref="OperationResult"/>.</param>
    /// <returns><c>true</c> if the deletion succeeded; otherwise <c>false</c>.</returns>
    protected async Task<bool> DeleteEntity(
        string entityName,
        Func<Task<OperationResult>> deleteAction)
    {
        var result = await deleteAction();

        if (result.IsSuccessful)
        {
            NotificationService.Success($"'{entityName}' has been deleted successfully.");
            return true;
        }
        
        NotificationService.Error($"Delete Failed, {result.Message}");
        return false;
    }

    /// <summary>
    /// Copies the specified text to the clipboard and shows a notification about the result.
    /// </summary>
    /// <param name="text">The text to copy.</param>
    protected void CopyToClipboard(string text)
    {
        try
        {
            NotificationService.Success($"'{text}' copied to clipboard");
        }
        catch (Exception)
        {
            NotificationService.Error("Failed to copy to clipboard");
        }
    }
    
    /// <summary>
    /// Opens URL in a new browser tab.
    /// </summary>
    protected async Task OpenInNewTab(string url)
    {
        await JsRuntime.InvokeVoidAsync("window.open", url, "_blank");
    }
}
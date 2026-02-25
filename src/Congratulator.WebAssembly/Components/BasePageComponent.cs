using Congratulator.WebAssembly.Models;
using Congratulator.WebAssembly.Services;
using Microsoft.AspNetCore.Components;

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
    public NotificationService NotificationService { get; set; } = null!;

    /// <summary>
    /// Executes a delete operation and shows a success or failure notification.
    /// </summary>
    /// <param name="entityName">Display name of the entity being deleted.</param>
    /// <param name="entityType">Type label shown in notifications (e.g. "Person").</param>
    /// <param name="deleteAction">Async function that performs the delete and returns an <see cref="OperationResult"/>.</param>
    /// <returns><c>true</c> if the deletion succeeded; otherwise <c>false</c>.</returns>
    protected async Task<bool> DeleteEntityWithConfirmation(
        string entityName,
        string entityType,
        Func<Task<OperationResult>> deleteAction)
    {
        var result = await deleteAction();

        if (result.IsSuccessful)
        {
            NotificationService.ShowSuccess("Deleted", $"{entityType} '{entityName}' has been deleted successfully.");
            return true;
        }
        
        NotificationService.ShowDestructive("Delete Failed", result.Message);
        return false;
    }

    /// <summary>
    /// Executes a bulk delete of other entity versions and shows a success or failure notification.
    /// </summary>
    /// <param name="entityName">Display name of the entity whose versions are being deleted.</param>
    /// <param name="entityType">Type label shown in notifications (e.g. "Config").</param>
    /// <param name="deleteAction">Async function that performs the delete and returns an <see cref="OperationResult{T}"/> with the count of deleted versions.</param>
    /// <returns><c>true</c> if the deletion succeeded; otherwise <c>false</c>.</returns>
    protected async Task<bool> DeleteEntityOtherVersionsWithConfirmation(
        string entityName,
        string entityType,
        Func<Task<OperationResult<int>>> deleteAction)
    {
        var result = await deleteAction();

        if (result.IsSuccessful)
        {
            NotificationService.ShowSuccess("Deleted",
                $"{result.Data} other version(s) of {entityType} '{entityName}' have been deleted successfully.");
            return true;
        }

        NotificationService.ShowDestructive("Delete Failed", result.Message);
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
            NotificationService.ShowSuccess("Copied to clipboard", $"'{text}' copied to clipboard");
        }
        catch (Exception)
        {
            NotificationService.ShowDestructive("Copy failed", "Failed to copy to clipboard");
        }
    }
}
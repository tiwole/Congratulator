using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.WebAssembly.Models;

namespace Congratulator.WebAssembly.Services;

public class NotificationService
{
    internal event Action<NotificationMessage> OnNotify = default!;

    /// <summary>
    /// Notifies subscribers with a notification message.
    /// </summary>
    /// <param name="message">The notification message to display.</param>
    public void Notify(NotificationMessage message) => OnNotify?.Invoke(message);

    /// <summary>
    /// Shows an info notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="description">Optional description text.</param>
    /// <param name="autoHide">Whether to auto-hide the notification.</param>
    /// <param name="delay">Delay in milliseconds before auto-hiding.</param>
    public void ShowInfo(string title, string? description = null, bool autoHide = true, int delay = 5000)
    {
        var message = new NotificationMessage(NotificationStatus.Info, title, description)
        {
            AutoHide = autoHide,
            Delay = delay
        };
        Notify(message);
    }

    /// <summary>
    /// Shows a success notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="description">Optional description text.</param>
    /// <param name="autoHide">Whether to auto-hide the notification.</param>
    /// <param name="delay">Delay in milliseconds before auto-hiding.</param>
    public void ShowSuccess(string title, string? description = null, bool autoHide = true, int delay = 5000)
    {
        var message = new NotificationMessage(NotificationStatus.Success, title, description)
        {
            AutoHide = autoHide,
            Delay = delay
        };
        Notify(message);
    }

    /// <summary>
    /// Shows a warning notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="description">Optional description text.</param>
    /// <param name="autoHide">Whether to auto-hide the notification.</param>
    /// <param name="delay">Delay in milliseconds before auto-hiding.</param>
    public void ShowWarning(string title, string? description = null, bool autoHide = true, int delay = 5000)
    {
        var message = new NotificationMessage(NotificationStatus.Warning, title, description)
        {
            AutoHide = autoHide,
            Delay = delay
        };
        Notify(message);
    }

    /// <summary>
    /// Shows a destructive/error notification.
    /// </summary>
    /// <param name="title">The notification title.</param>
    /// <param name="description">Optional description text.</param>
    /// <param name="autoHide">Whether to auto-hide the notification.</param>
    /// <param name="delay">Delay in milliseconds before auto-hiding.</param>
    public void ShowDestructive(string title, string? description = null, bool autoHide = true, int delay = 5000)
    {
        var message = new NotificationMessage(NotificationStatus.Destructive, title, description)
        {
            AutoHide = autoHide,
            Delay = delay
        };
        Notify(message);
    }
}
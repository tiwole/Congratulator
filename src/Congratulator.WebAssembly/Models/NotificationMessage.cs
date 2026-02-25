using Congratulator.SharedKernel.Contracts.Enums;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Models;

public class NotificationMessage : IEquatable<NotificationMessage>
{
     public NotificationMessage()
    {
        Id = Guid.NewGuid();
    }

    public NotificationMessage(NotificationStatus status, string title, string? description = null)
    {
        Id = Guid.NewGuid();
        Status = status;
        Title = title;
        Description = description;
        Size = string.IsNullOrWhiteSpace(description)
            ? ElementSize.Small
            : ElementSize.Medium;
    }

    /// <summary>
    /// Unique identifier for the notification.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets or sets the notification status/type.
    /// </summary>
    /// <remarks>
    /// Default value is Info.
    /// </remarks>
    public NotificationStatus Status { get; set; } = NotificationStatus.Info;

    /// <summary>
    /// Gets or sets the size of the notification.
    /// </summary>
    /// <remarks>
    /// Default value is Medium.
    /// </remarks>
    public ElementSize Size { get; set; } = ElementSize.Medium;

    /// <summary>
    /// Gets or sets the notification title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the notification description (only for medium size).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets custom lead icon content.
    /// </summary>
    public RenderFragment? LeadIcon { get; set; }

    /// <summary>
    /// Gets or sets child content for action buttons.
    /// </summary>
    public RenderFragment? Actions { get; set; }

    /// <summary>
    /// Gets or sets whether the notification should automatically hide.
    /// </summary>
    /// <remarks>
    /// Default value is true.
    /// </remarks>
    public bool AutoHide { get; set; } = true;

    /// <summary>
    /// Gets or sets the delay in milliseconds before hiding the notification.
    /// </summary>
    /// <remarks>
    /// Default value is 5000ms (5 seconds).
    /// </remarks>
    public int Delay { get; set; } = 5000;

    /// <summary>
    /// Gets or sets whether to show the leading icon.
    /// </summary>
    /// <remarks>
    /// Default value is true.
    /// </remarks>
    public bool ShowLeadIcon { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show the close button.
    /// </summary>
    /// <remarks>
    /// Default value is true.
    /// </remarks>
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Internal callback for hiding the notification with animation.
    /// Set by the Notification component.
    /// </summary>
    internal Func<Task>? HideCallback { get; set; }

    public bool Equals(NotificationMessage? other) => other != null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as NotificationMessage);

    public override int GetHashCode() => Id.GetHashCode();
}
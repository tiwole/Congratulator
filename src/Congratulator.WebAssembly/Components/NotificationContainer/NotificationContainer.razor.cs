using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.WebAssembly.Models;
using Congratulator.WebAssembly.Services;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.NotificationContainer;

/// <summary>
/// Container component that subscribes to <see cref="NotificationService"/>
/// and renders toast notifications.
/// </summary>
public partial class NotificationContainer : ComponentBase, IDisposable
{
    [Inject]
    private NotificationService NotificationService { get; set; } = null!;

    private readonly List<NotificationMessage> _notifications = [];
    private readonly Dictionary<Guid, Timer> _timers = [];

    protected override void OnInitialized()
    {
        NotificationService.OnNotify += OnNotify;
    }

    private void OnNotify(NotificationMessage message)
    {
        InvokeAsync(() =>
        {
            _notifications.Add(message);
            StateHasChanged();

            if (message.AutoHide)
            {
                var timer = new Timer(_ => InvokeAsync(() => RemoveNotification(message)), null, message.Delay, Timeout.Infinite);
                _timers[message.Id] = timer;
            }
        });
    }

    private void RemoveNotification(NotificationMessage message)
    {
        _notifications.Remove(message);

        if (_timers.Remove(message.Id, out var timer))
        {
            timer.Dispose();
        }

        StateHasChanged();
    }

    public void Dispose()
    {
        NotificationService.OnNotify -= OnNotify;

        foreach (var timer in _timers.Values)
        {
            timer.Dispose();
        }

        _timers.Clear();
    }
}
using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Congratulator.WebAssembly.Components.HomeSection;

public partial class HomeSection : BasePageComponent, IDisposable
{
    [Parameter, EditorRequired] public string Title { get; set; } = null!;
    [Parameter] public List<PersonModel> People { get; set; } = [];
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback<PersonModel> OnPersonClick { get; set; }

    private readonly int _skeletonCount = Random.Shared.Next(1, 7);
    private bool _isVisible;
    private bool _observerRegistered;
    private ElementReference _sentinel;
    private DotNetObjectReference<HomeSection>? _dotNetRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!IsLoading && !_isVisible && !_observerRegistered && People.Count > 0)
        {
            _observerRegistered = true;
            _dotNetRef = DotNetObjectReference.Create(this);
            await JsRuntime.InvokeVoidAsync("observeElement", _sentinel, _dotNetRef, "300px");
        }
    }

    [JSInvokable]
    public void OnVisible()
    {
        _isVisible = true;
        StateHasChanged();
    }

    public void Dispose() => _dotNetRef?.Dispose();
}
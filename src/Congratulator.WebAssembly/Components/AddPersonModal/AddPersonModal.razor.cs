using System.Net.Http.Json;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.WebAssembly.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Congratulator.WebAssembly.Components.AddPersonModal;

public partial class AddPersonModal : ComponentBase
{
    #region Injected Services
    [Inject]
    public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject]
    public DateTimeProvider DateTimeProvider { get; set; } = null!;
    #endregion
    
    #region Parameters
    [Parameter]
    public bool IsVisible { get; set; }

    [Parameter]
    public EventCallback<bool> IsVisibleChanged { get; set; }

    [Parameter]
    public EventCallback<CreatePersonRequest> OnPersonAdded { get; set; }
    #endregion

    #region Form State
    // ReSharper disable once InconsistentNaming
    private IBrowserFile? SelectedFile;
    private string? PhotoUrl { get; set; }
    private string FirstName { get; set; } = string.Empty;
    private string? LastName { get; set; } = string.Empty;
    private DateOnly? BirthDate { get; set; }
    private string SelectedRelationship { get; set; } = string.Empty;
    #endregion

    #region Validation
    private Dictionary<string, string> Errors { get; set; } = new();

    private bool HasError(string field) => Errors.ContainsKey(field);
    private string GetError(string field) => Errors.TryGetValue(field, out var msg) ? msg : string.Empty;
    private void ClearError(string field) => Errors.Remove(field);

    private bool Validate()
    {
        Errors.Clear();

        if (string.IsNullOrWhiteSpace(FirstName))
            Errors[nameof(FirstName)] = "First name is required";
        if (FirstName.Length > 32)
            Errors[nameof(FirstName)] = "First name cannot exceed 32 characters";
        
        if (!string.IsNullOrWhiteSpace(LastName) && LastName.Length > 64)
            Errors[nameof(LastName)] = "Last name cannot exceed 64 characters";

        if (BirthDate is null)
            Errors[nameof(BirthDate)] = "Date of birth is required";
        else if (BirthDate.Value > DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            Errors[nameof(BirthDate)] = "Date of birth cannot be in the future";
        else if (BirthDate.Value < DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-120)))
            Errors[nameof(BirthDate)] = "Please enter a valid date";

        return Errors.Count == 0;
    }
    #endregion

    #region Handlers
    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var imageFile = e.File;
        var format = "image/png";
        var resizedImage = await imageFile.RequestImageFileAsync(format, 400, 400);

        using var ms = new MemoryStream();
        await resizedImage.OpenReadStream().CopyToAsync(ms);

        PhotoUrl = $"data:{format};base64,{Convert.ToBase64String(ms.ToArray())}";
        
        SelectedFile = resizedImage;
    }
    
    private async Task OnSubmit()
    {
        if (!Validate())
            return;

        RelationshipType? relationship = null;
        string? photo = null;

        if (SelectedFile != null)
        {
            using var stream = SelectedFile.OpenReadStream(2 * 1024 * 1024);
            var bytes = new byte[stream.Length];
            await stream.ReadExactlyAsync(bytes);
            
            photo = Convert.ToBase64String(bytes);
        }

        if (relationship != null)
        {
            relationship = Enum.Parse<RelationshipType>(SelectedRelationship); 
        }
        
        var request = new CreatePersonRequest
        {
            FirstName = FirstName.Trim(),
            LastName = LastName ?? string.Empty,
            BirthDate = BirthDate!.Value,
            RelationshipType = relationship,
            Photo = photo
        };

        var client = HttpClientFactory.CreateClient("ApiClient");
        await client.PostAsJsonAsync(Routes.Api.Persons, request, CancellationToken.None);

        await OnPersonAdded.InvokeAsync(request);
        ResetForm();
        await Close();
    }

    private async Task Close()
    {
        ResetForm();
        await IsVisibleChanged.InvokeAsync(false);
    }

    private async Task OnOverlayClick()
    {
        await Close();
    }

    private void ResetForm()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        PhotoUrl = string.Empty;
        BirthDate = null;
        SelectedRelationship = string.Empty;
        Errors.Clear();
    }
    #endregion
}

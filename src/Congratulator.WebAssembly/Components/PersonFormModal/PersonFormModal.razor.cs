using System.Net.Http.Json;
using Blazor.Sonner.Services;
using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using Congratulator.SharedKernel.Contracts.Models.Requests;
using Congratulator.WebAssembly.Services;
using LumexUI;
using LumexUI.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Congratulator.WebAssembly.Components.PersonFormModal;

public partial class PersonFormModal : BasePageComponent
{
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Parameter] public EventCallback OnPersonSaved { get; set; }

    private LumexModal _modal = null!;
    private bool _isEditMode;
    private Guid _editingPersonId;

    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private DateOnly? _birthDate;
    private RelationshipType _relationship = RelationshipType.Unknown;
    private string? _photo;

    private Dictionary<string, string> _errors = new();
    private bool HasError(string key) => _errors.ContainsKey(key);
    private string GetError(string key) => _errors.TryGetValue(key, out var msg) ? msg : string.Empty;
    private void ClearError(string key) => _errors.Remove(key);

    private bool IsFormValid =>
        !string.IsNullOrWhiteSpace(_firstName) && _birthDate.HasValue;

    public async Task OpenCreate()
    {
        _isEditMode = false;
        ResetForm();
        await _modal.OpenAsync();
    }

    public async Task OpenEdit(PersonModel person)
    {
        _isEditMode = true;
        _editingPersonId = person.Id;
        _firstName = person.FirstName;
        _lastName = person.LastName ?? string.Empty;
        _birthDate = person.BirthDate;
        _relationship = person.RelationshipType;
        _photo = person.PhotoPath;
        _errors.Clear();
        StateHasChanged();
        await _modal.OpenAsync();
    }

    private async Task CloseAsync()
    {
        await _modal.CloseAsync();
    }

    private void OnModalClosed()
    {
        ResetForm();
    }

    private void ResetForm()
    {
        _firstName = string.Empty;
        _lastName = string.Empty;
        _birthDate = null;
        _relationship = RelationshipType.Unknown;
        _photo = null;
        _errors.Clear();
    }

    private async Task HandleFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        const string format = "image/png";
        var resized = await file.RequestImageFileAsync(format, 400, 400);
        using var ms = new MemoryStream();
        await resized.OpenReadStream(2 * 1024 * 1024).CopyToAsync(ms);
        _photo = $"data:{format};base64,{Convert.ToBase64String(ms.ToArray())}";
    }

    private async Task SubmitAsync()
    {
        if (!Validate()) return;

        var client = HttpClientFactory.CreateClient("ApiClient");

        if (_isEditMode)
        {
            var request = new UpdatePersonRequest
            {
                FirstName = _firstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(_lastName) ? null : _lastName.Trim(),
                BirthDate = _birthDate,
                RelationshipType = _relationship,
                Photo = ExtractBase64(_photo)
            };
            var response = await client.PutAsJsonAsync($"{Routes.Api.Persons}/{_editingPersonId}", request);
            if (!response.IsSuccessStatusCode)
            {
                NotificationService.Error("Failed to update person.");
                return;
            }

            var name = string.IsNullOrEmpty(_lastName) ? _firstName : $"{_firstName} {_lastName}";
            NotificationService.Success($"'{name}' updated successfully.");
        }
        else
        {
            var request = new CreatePersonRequest
            {
                FirstName = _firstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(_lastName) ? null : _lastName.Trim(),
                BirthDate = _birthDate!.Value,
                RelationshipType = _relationship.ToString(),
                Photo = ExtractBase64(_photo)
            };
            var response = await client.PostAsJsonAsync(Routes.Api.Persons, request);
            if (!response.IsSuccessStatusCode)
            {
                NotificationService.Error("Failed to create person.");
                return;
            }
            NotificationService.Success($"'{_firstName}' added successfully.");
        }

        await _modal.CloseAsync();
        await OnPersonSaved.InvokeAsync();
    }

    private static string? ExtractBase64(string? photo)
    {
        if (photo == null || !photo.StartsWith("data:")) return null;
        var comma = photo.IndexOf(',');
        return comma >= 0 ? photo[(comma + 1)..] : null;
    }

    private bool Validate()
    {
        _errors.Clear();

        if (string.IsNullOrWhiteSpace(_firstName))
            _errors["FirstName"] = "First name is required";
        else if (_firstName.Length > 32)
            _errors["FirstName"] = "First name cannot exceed 32 characters";

        if (!string.IsNullOrWhiteSpace(_lastName) && _lastName.Length > 64)
            _errors["LastName"] = "Last name cannot exceed 64 characters";

        if (_birthDate is null)
            _errors["BirthDate"] = "Date of birth is required";
        else if (_birthDate.Value > DateOnly.FromDateTime(DateTimeProvider.UtcNow.AddDays(1)))
            _errors["BirthDate"] = "Date of birth cannot be in the future";
        else if (_birthDate.Value < DateOnly.FromDateTime(DateTimeProvider.UtcNow.AddYears(-120)))
            _errors["BirthDate"] = "Please enter a valid date";

        return _errors.Count == 0;
    }
}
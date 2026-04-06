using Congratulator.SharedKernel.Contracts.Enums;
using Congratulator.SharedKernel.Contracts.Models;
using LumexUI;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.PersonCard;

public partial class PersonCard : ComponentBase
{
    [Parameter] public PersonModel Person { get; set; } = null!;

    private readonly CardSlots _cardClasses = new()
    {
        Base = "shadow-none border-none bg-transparent"
    };

    private readonly AvatarSlots _avatarClasses = new()
    {
        Base = "shrink-0"
    };

    private string FullName => string.IsNullOrWhiteSpace(Person.LastName)
        ? Person.FirstName
        : $"{Person.FirstName} {Person.LastName}";

    private string FormattedBirthDate =>
        Person.BirthDate.ToString("dd MMM yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

    private string RelationshipIcon => Person.RelationshipType switch
    {
        RelationshipType.Friend => "ri-user-heart-line",
        RelationshipType.Mate => "ri-user-line",
        RelationshipType.Coworker => "ri-briefcase-line",
        RelationshipType.Family => "ri-home-heart-line",
        /* Unknown */ _ => "ri-question-line"
    };

    private ChipSlots RelationshipChipClasses => Person.RelationshipType switch
    {
        RelationshipType.Friend => new ChipSlots
        {
            Base = "bg-blue-100 text-blue-600",
            Content = "text-blue-600 font-semibold"
        },
        RelationshipType.Mate => new ChipSlots
        {
            Base = "bg-pink-100 text-pink-600",
            Content = "text-pink-600 font-semibold"
        },
        RelationshipType.Coworker => new ChipSlots
        {
            Base = "bg-green-100 text-green-600",
            Content = "text-green-600 font-semibold"
        },
        RelationshipType.Family => new ChipSlots
        {
            Base = "bg-amber-100 text-amber-700",
            Content = "text-amber-700 font-semibold"
        },
        _ => new ChipSlots
        {
            Base = "bg-gray-100 text-gray-500",
            Content = "text-gray-500 font-semibold"
        }
    };

    private bool IsBirthdayToday => Person.DaysUntilBirthday == 0;

    private string CountdownModifier => Person.DaysUntilBirthday switch
    {
        0 => "countdown--today",
        <= 7 => "countdown--soon",
        _ => ""
    };

    private string CountdownText => Person.DaysUntilBirthday switch
    {
        0 => "Birthday today!",
        1 => "Tomorrow!",
        _ => $"{Person.DaysUntilBirthday} days left"
    };

    private string AgeText => Person.Age == 1 ? "1 year" : $"{Person.Age} years";
}
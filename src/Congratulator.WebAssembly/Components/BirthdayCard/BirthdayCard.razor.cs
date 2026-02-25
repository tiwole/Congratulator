using Congratulator.SharedKernel.Contracts.Models;
using Microsoft.AspNetCore.Components;

namespace Congratulator.WebAssembly.Components.BirthdayCard;

public partial class BirthdayCard : ComponentBase
{
    #region Parameters
    [Parameter] public PersonModel Person { get; set; } = null!;
    #endregion

    #region Word Methods
    private static string GetAgeWord(int age)
    {
        return age == 1 ? "year old" : "years old";
    }

    private static string GetDaysWord(int days)
    {
        return days == 1 ? "day" : "days";
    }

    private string GetStatusClass()
    {
        return Person.DaysUntilBirthday switch
        {
            0 => "status-today",
            <= 1 => "status-soon",
            _ => "status-normal"
        };
    }
    #endregion
}
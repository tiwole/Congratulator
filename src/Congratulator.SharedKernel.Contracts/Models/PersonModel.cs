using Congratulator.SharedKernel.Contracts.Enums;

namespace Congratulator.SharedKernel.Contracts.Models;

public class PersonModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public RelationshipType RelationshipType { get; set; }
    public string? PhotoPath { get; set; }
    public int Age => DateTime.Today.Year - BirthDate.Year - (DateTime.Today.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
    public DateOnly NextBirthday
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var birthdayThisYear = BirthDate.AddYears(today.Year - BirthDate.Year);
            
            return birthdayThisYear < today 
                ? birthdayThisYear.AddYears(1) 
                : birthdayThisYear;
        }
    }
    public int DaysUntilBirthday
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return NextBirthday.DayNumber - today.DayNumber;
        }
    }
}
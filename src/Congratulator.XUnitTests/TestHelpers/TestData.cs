using System;
using System.Collections.Generic;
using Congratulator.SharedKernel.Entities;

namespace Congratulator.XUnitTests.TestHelpers;

public static class TestData
{
    public static List<Person> GetTestPersons() => new()
    {
        new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", BirthDate = new DateOnly(1990, 1, 1) },
        new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", BirthDate = new DateOnly(1985, 5, 15) },
        new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", BirthDate = new DateOnly(1995, 10, 20) }
    };
}

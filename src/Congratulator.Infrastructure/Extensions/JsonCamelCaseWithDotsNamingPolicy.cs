using System.Text.Json;

namespace Congratulator.Infrastructure.Extensions;

public class JsonCamelCaseWithDotsNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        string[] source = name.Split('.');
        for (int index = 0; index < source.Length; ++index)
            source[index] = JsonNamingPolicy.CamelCase.ConvertName(source[index]);
        return !((IEnumerable<string>) source).Any<string>() ? source.ToString() : string.Join(".", source);
    }
}
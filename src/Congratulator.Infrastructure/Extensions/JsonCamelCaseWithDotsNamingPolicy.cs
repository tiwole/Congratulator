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
            source[index] = CamelCase.ConvertName(source[index]);
        return (source.Length == 0 ? source.ToString() : string.Join(".", source))!;
    }
}
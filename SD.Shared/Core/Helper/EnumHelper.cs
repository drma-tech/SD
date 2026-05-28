namespace SD.Shared.Core.Helper;

public static class EnumHelper
{
    public static TEnum[] GetArray<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues<TEnum>();
    }

    public static List<EnumObject<TEnum>> GetList<TEnum>(bool translate = true) where TEnum : struct, Enum
    {
        var result = new List<EnumObject<TEnum>>();
        foreach (var val in GetArray<TEnum>())
        {
            var attr = val.GetCustomAttribute(translate);

            result.Add(new EnumObject<TEnum>(val, attr?.Name, attr?.Description, attr?.Group));
        }
        return result;
    }

    public static TEnum ParseToEnum<TEnum>(this string? value) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result) && Enum.IsDefined(result))
        {
            return result;
        }
        else
        {
            throw new ArgumentException($"Invalid value for enum type {typeof(TEnum).Name}: {value}");
        }
    }

    public static TEnum ParseToEnum<TEnum>(this string? value, TEnum fallback) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value, true, out var result) && Enum.IsDefined(result))
        {
            return result;
        }

        return fallback;
    }
}

public class EnumObject<TEnum>(TEnum value, string? name, string? description, string? group) where TEnum : struct, Enum
{
    public TEnum Value { get; set; } = value;
    public string? Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public string? Group { get; set; } = group;
}
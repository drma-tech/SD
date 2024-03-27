namespace SD.Shared.Core.Helper
{
    public static class EnumHelper
    {
        public static TEnum[] GetArray<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>();
        }

        public static EnumObject[] GetObjArray<TEnum>(bool translate = true) where TEnum : struct, Enum
        {
            return GetList<TEnum>(translate).ToArray();
        }

        public static IEnumerable<EnumObject> GetList<TEnum>(bool translate = true) where TEnum : struct, Enum
        {
            foreach (var val in GetArray<TEnum>())
            {
                var attr = val.GetCustomAttribute(translate);

                yield return new EnumObject(val, attr?.Name, attr?.Description, attr?.Group);
            }
        }
    }

    public class EnumObject
    {
        public EnumObject(Enum value, string? name, string? description, string? group)
        {
            Value = value;
            Name = name;
            Description = description;
            Group = group;
        }

        public Enum Value { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
    }
}
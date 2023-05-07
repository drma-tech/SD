namespace SD.Shared.Helper
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
        public EnumObject(Enum Value, string? Name, string? Description, string? Group)
        {
            this.Value = Value;
            this.Name = Name;
            this.Description = Description;
            this.Group = Group;
        }

        public Enum Value { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Group { get; set; }
    }
}
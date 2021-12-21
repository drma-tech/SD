using SD.Shared.Core;

namespace SD.Shared.Helper
{
    public static class EnumHelper
    {
        public static TEnum[] GetArray<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>();
        }

        public static IEnumerable<EnumObject> GetList<TEnum>(bool translate = true) where TEnum : struct, Enum
        {
            foreach (var val in GetArray<TEnum>())
            {
                var attr = ((Enum)val).GetCustomAttribute(translate);

                yield return new EnumObject(Convert.ToInt32(val), val, attr.Name, attr.Description);
            }
        }
    }

    public class EnumObject
    {
        public EnumObject(int Value, object ValueObject, string Name, string Description)
        {
            this.Value = Value;
            this.ValueObject = ValueObject;
            this.Name = Name;
            this.Description = Description;
        }

        public int Value { get; set; }
        public object ValueObject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
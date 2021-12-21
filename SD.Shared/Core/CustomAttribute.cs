using SD.Shared.Core;
using System.Reflection;
using System.Resources;

namespace SD.Shared.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CustomAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Translations resource file
        /// </summary>
        public Type ResourceType { get; set; }
    }

    public static class CustomAttributeHelper
    {
        public static string GetName(this Enum value, bool translate = true)
        {
            return value.GetCustomAttribute(translate)?.Name;
        }

        public static string GetDescription(this Enum value, bool translate = true)
        {
            return value.GetCustomAttribute(translate)?.Description;
        }

        public static CustomAttribute? GetCustomAttribute(this Enum value, bool translate = true)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null) return default;

            var attr = fieldInfo.GetCustomAttribute(typeof(CustomAttribute)) as CustomAttribute;

            if (attr == null) throw new NullReferenceException("attr null");

            if (translate && attr.ResourceType != null) //translations
            {
                var rm = new ResourceManager(attr.ResourceType.FullName ?? "", attr.ResourceType.Assembly);

                if (rm == null) throw new NullReferenceException("ResourceManager null");

                if (!string.IsNullOrEmpty(attr.Name)) attr.Name = rm.GetString(attr.Name);
                if (!string.IsNullOrEmpty(attr.Description)) attr.Description = rm.GetString(attr.Description);
            }

            return attr;
        }

        public static CustomAttribute? GetCustomAttribute(this Type type, string? name = null)
        {
            if (type == null) return default;

            CustomAttribute attr;

            if (string.IsNullOrEmpty(name))
            {
                attr = type.GetCustomAttribute(typeof(CustomAttribute)) as CustomAttribute;
            }
            else
            {
                var property = type.GetProperty(name) as MemberInfo;
                attr = property.GetCustomAttribute(typeof(CustomAttribute)) as CustomAttribute;
            }

            if (attr.ResourceType != null) //translations
            {
                var rm = new ResourceManager(attr.ResourceType.FullName, attr.ResourceType.Assembly);

                if (!string.IsNullOrEmpty(attr.Name)) attr.Name = rm.GetString(attr.Name);
                if (!string.IsNullOrEmpty(attr.Description)) attr.Description = rm.GetString(attr.Description);
            }

            return attr;
        }

        //public static Dictionary<string, object> GetMatBlazorAttributes(Type type, string propertyName = null)
        //{
        //    if (type == null) return default;

        //    var Annotations = new Dictionary<string, object>();

        //    if (string.IsNullOrEmpty(propertyName))
        //    {
        //        if (type.GetCustomAttribute(typeof(MatBlazorAttribute)) is MatBlazorAttribute matBlazorAttribute)
        //        {
        //            Annotations = matBlazorAttribute.GetAttributes();
        //        }
        //    }
        //    else
        //    {
        //        var property = type.GetProperty(propertyName) as MemberInfo;
        //        if (property.GetCustomAttribute(typeof(MatBlazorAttribute)) is MatBlazorAttribute matBlazorAttribute)
        //        {
        //            Annotations = matBlazorAttribute.GetAttributes();
        //        }
        //    }

        //    if (!Annotations.ContainsKey("Label") && !string.IsNullOrEmpty(GetName(type, propertyName)))
        //    {
        //        Annotations.TryAdd("Label", GetName(type, propertyName));
        //    }

        //    if (!Annotations.ContainsKey("PlaceHolder") && !string.IsNullOrEmpty(GetPrompt(type, propertyName)))
        //    {
        //        Annotations.TryAdd("PlaceHolder", GetPrompt(type, propertyName));
        //    }

        //    //TODO: when there is a helptext, it is generating a line break in the form
        //    if (!Annotations.ContainsKey("HelperText") && !string.IsNullOrEmpty(GetDescription(type, propertyName)))
        //    {
        //        Annotations.TryAdd("HelperText", GetDescription(type, propertyName));
        //    }

        //    return Annotations;
        //}
    }
}
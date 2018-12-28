using System;
using System.ComponentModel;
using System.Reflection;

namespace METU.VRS.UI
{
    public static class Util
    {
        public static string ToDescription<TEnum>(this TEnum EnumValue) where TEnum : struct
        {
            return ToDescription((Enum)(object)EnumValue);
        }

        public static string ToDescription<TEnum>(this TEnum? EnumValue) where TEnum : struct
        {
            if (EnumValue.HasValue)
            {
                return ToDescription((Enum)(object)EnumValue);
            }
            else
            {
                return "";
            }

        }

        public static string ToDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
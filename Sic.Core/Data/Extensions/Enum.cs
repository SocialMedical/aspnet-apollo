using Sic.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sic
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the description attribute for the enum
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum e)
        {
            var da = (DescriptionAttribute[])(e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false));

            return da.Length > 0 ? da[0].Description : e.ToString();
        }

        public static string GetDisplay(this Enum e)
        {
            var da = (DisplayAttribute[])(e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false));

            return da.Length > 0 ? da[0].GetName() : e.ToString();
        }

        public static string GetID(this Enum value)
        {
            Type type = value.GetType();

            object item = Enum.Parse(type, value.ToString());

            FieldInfo field = type.GetField(item.ToString());

            if (field != null)
            {
                IDAttribute attrs = (IDAttribute)field.GetCustomAttributes(typeof(IDAttribute), false).FirstOrDefault();

                if (attrs != null)
                    return attrs.IdValue;
                else
                    return String.Empty;
            }
            else
                return String.Empty;
        }
    }
}

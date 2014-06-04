using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sic
{
    public static class ObjectExtensions
    {
        public static string GetDisplay<T>(this T value, Type type)
        {
            object item = Enum.Parse(type, value.ToString());

            FieldInfo field = type.GetField(item.ToString());

            if (field != null)
            {
                DisplayAttribute attrs = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

                if (attrs != null)
                    return attrs.GetName();
                else
                    return String.Empty;
            }
            else
                return String.Empty;
        }        
        
    }


}

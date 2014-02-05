using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sic.Data
{    
    public class Parameter
    {
        public Parameter()
        {

        }

        public Parameter(string name,string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; set; }
        public string Value { get; set; }

        public static List<Parameter> GetPropertiesParameters(object entity, string[] exludeProperties = null)
        {
            List<Parameter> parameters = new List<Parameter>();
            var listProperties = entity.GetType().GetProperties().ToList();
            foreach (PropertyInfo info in listProperties)
            {                                
                if (exludeProperties == null || !exludeProperties.Contains(info.Name))
                {
                    object valueProperty = info.GetValue(entity, null);
                    string value = string.Empty;

                    if (valueProperty == null || (valueProperty.GetType().Name != typeof(String).Name 
                        && !valueProperty.GetType().IsValueType))
                        value = "NULL";
                    else
                    {
                        if (valueProperty.GetType().Name == typeof(bool).Name)
                            value = Convert.ToInt32(valueProperty).ToString();
                        else if (valueProperty.GetType().Name == typeof(String).Name)
                            value = string.Format("'{0}'", valueProperty.ToString());
                        else if (valueProperty.GetType().Name == typeof(DateTime).Name)
                            value = string.Format("'{0}{1:00}{2:00} {3}:{4}:{5}'", ((DateTime)valueProperty).Year, ((DateTime)valueProperty).Month,
                                ((DateTime)valueProperty).Day, ((DateTime)valueProperty).Hour, 
                                ((DateTime)valueProperty).Minute, ((DateTime)valueProperty).Second);
                        else
                            value = valueProperty.ToString();
                    }

                    Parameter parameter = new Parameter(string.Format("@{0}", info.Name), value);

                    parameters.Add(parameter);
                }
            }
            return parameters;
        }

        public static string ApplyParameters(string pattern, List<Parameter> parameters)
        {
            string value = pattern;
            if (parameters != null)
            {
                foreach (Parameter parameter in parameters)
                    value = value.Replace(parameter.Name, parameter.Value);
            }
            return value;
        }
    }
}

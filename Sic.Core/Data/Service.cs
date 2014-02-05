﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sic.Data.Entity;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sic.Data
{
    public static class Service
    {
        public static string GetDefaultFullName(INameable nameable)
        {
            return string.Format("{0} {1}", nameable.Names, nameable.LastNames);
        }

        private static object GetItem(IEnumerable list, int key)
        {
            foreach (object item in list)
            {
                IIdentifiable identifiable = item as IIdentifiable;
                if (identifiable != null && identifiable.Key == key) return item;
            }
            return null;
        }

        private static Type GetItemType(object list)
        {
            foreach (Type interfaceType in list.GetType().GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return list.GetType().GetGenericArguments()[0];
                }
            }
            return null;
        }

        public static object ConvertTo(this object source, Type targetType)
        {
            object targetObject = Activator.CreateInstance(targetType);
            source.CopyTo(targetObject, includeCollectionProperties: false);

            return targetObject;
        }

        public static object CopyTo(this IIdentifiable entitySource, IEnumerable<IIdentifiable> findTargetCollection,
            string[] includeProperties = null, string[] excludeProperties = null)
        {
            object targetUpdate = findTargetCollection.FirstOrDefault(p => p.Key == entitySource.Key);
            if (targetUpdate != null)
                entitySource.CopyTo(targetUpdate, includeProperties: includeProperties, excludeProperties: excludeProperties);

            return targetUpdate;
        }

        public static bool CopyTo(this object source, object target,
            bool includeCollectionProperties = false, string[] includeProperties = null, string[] excludeProperties = null)
        {
            Type targetType = target.GetType();
            //object returnValue = target;
            //if (target == null)
            //    returnValue = Activator.CreateInstance(targetType);
            //else
            //{
            //    EditableBase editable = source as EditableBase;
            //    if (editable != null) editable.IsNew = false;
            //}

            if (source != null)
            {
                foreach (PropertyInfo sourceInfo in source.GetType().GetProperties())
                {
                    if ((includeProperties == null || includeProperties.Contains(sourceInfo.Name))
                        && (excludeProperties == null || !excludeProperties.Contains(sourceInfo.Name)))
                    {
                        PropertyInfo targetInfo = targetType.GetProperty(sourceInfo.Name);

                        if (targetInfo != null && targetInfo.CanWrite && sourceInfo.CanRead)
                        {
                            object sourceValue = sourceInfo.GetValue(source, null);
                            try
                            {
                                object targetValue = targetInfo.GetValue(target, null);

                                IList collSource = sourceValue as IList;
                                IEnumerable collTarget = targetValue as IEnumerable;
                                if (collSource != null && collTarget != null)
                                {
                                    if (includeCollectionProperties)
                                    {
                                        foreach (object itemSource in collSource)
                                        {
                                            IIdentifiable identifiable = itemSource as IIdentifiable;
                                            if (identifiable != null)
                                            {
                                                object itemTarget = GetItem(collTarget, identifiable.Key);
                                                itemTarget = CopyTo(itemSource, itemTarget);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //    //Determine Foreign Key Property
                                    //    ForeignKeyAttribute foreignKeyAttribute =
                                    //        targetInfo.GetCustomAttributes(typeof(ForeignKeyAttribute), true)[0] as ForeignKeyAttribute;

                                    //    if (foreignKeyAttribute != null)
                                    //    {
                                    //        string navigationProperty = foreignKeyAttribute.Name;

                                    //    }

                                    targetInfo.SetValue(target, sourceValue, null);
                                }
                            }
                            catch { return false; }
                        }
                    }
                }
            }
            return true;
        }
    }
}

namespace Sic
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the description attribute for the enum
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string Description(this Enum e)
        {
            var da = (DescriptionAttribute[])(e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false));

            return da.Length > 0 ? da[0].Description : e.ToString();
        }

    }

    public static class Extensions
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
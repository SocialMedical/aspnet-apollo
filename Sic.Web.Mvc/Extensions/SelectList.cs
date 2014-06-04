using Sic.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Sic.Web.Mvc
{
    public static class SelectListExtensions
    {
        public static SelectList ToSelectList<TEntity>(this IEnumerable<TEntity> collection,
            Func<TEntity, bool> selected = null, string valueMember = null, string displayMember = null,
            bool includeNullItem = false
            ) where TEntity : EntityBase
        {
            var list = collection.ToList();

            if (includeNullItem)
            {
                TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
                list.Insert(0, entity);
            }

            TEntity selectedValue = null;
            if (selected != null)
                selectedValue = list.Where(selected).SingleOrDefault();

            if (string.IsNullOrEmpty(valueMember))
                valueMember = "Key";
            if (string.IsNullOrEmpty(displayMember))
                displayMember = "DescriptionName";

            return new SelectList(list, valueMember, displayMember, selectedValue == null ? null : (object)selectedValue.Key);
        }

        public static SelectList ToSelectList<TEntity>(this Repository<TEntity> repository,
            Expression<Func<TEntity, bool>> filter = null,
            Func<TEntity, bool> selected = null, string valueMember = null, string displayMember = null, bool includeNullItem = false) where TEntity : EntityBase
        {
            var list = repository.Get(filter).OrderBy(p => p.DescriptionName).ToList();
            return ToSelectList(list, selected, includeNullItem: includeNullItem);
        }

        public static SelectList ToSelectList(this Enum enumeration)
        {
            var source = Enum.GetValues(enumeration.GetType());

            var items = new Dictionary<int, string>();

            foreach (var value in source)
            {
                FieldInfo field = value.GetType().GetField(value.ToString());

                DisplayAttribute attrs = (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
                if (attrs != null)
                {
                    items.Add(Convert.ToInt32(value), attrs.GetName());
                }
                else
                {
                    DescriptionAttribute attr2s = (DescriptionAttribute)field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                    if (attr2s != null)
                        items.Add(Convert.ToInt32(value), attr2s.Description);
                }

            }

            return new SelectList(items, "Key", "Value", (Convert.ToInt32(enumeration)));
        }
    }
}

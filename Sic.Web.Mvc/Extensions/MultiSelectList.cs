using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using Sic.Data.Entity;

namespace Sic.Web.Mvc
{
    public static class MultiSelectListExtensions
    {
        public static MultiSelectList ToMultiSelectList<TEntity>(this Repository<TEntity> repository,
            Expression<Func<TEntity, bool>> filter = null, IEnumerable selectedValues = null,
            string valueMember = null, string displayMember = null) where TEntity : EntityBase
        {
            var list = repository.Get(filter).ToList();
            if (string.IsNullOrEmpty(valueMember))
                valueMember = "Key";
            if (string.IsNullOrEmpty(displayMember))
                displayMember = "DescriptionName";
            return new MultiSelectList(list, valueMember, displayMember, selectedValues);
        }

        public static MultiSelectList ToMultiSelectList<TEntity>(this IEnumerable<TEntity> collection,
            IEnumerable selectedValues = null, string valueMember = null, string displayMember = null) where TEntity : EntityBase
        {
            var list = collection.ToList();

            if (string.IsNullOrEmpty(valueMember))
                valueMember = "Key";
            if (string.IsNullOrEmpty(displayMember))
                displayMember = "DescriptionName";

            return new MultiSelectList(list, valueMember, displayMember, selectedValues);
        }
    }
}

using System.Web.Mvc;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Sic.Data.Entity;
using System.Linq.Expressions;
using System.Web.Routing;
using System.Collections;
using System.Globalization;
using System.ComponentModel;

namespace System.Web.Mvc
{ 
    public static class Extensions
    {        
        public static string ContentVersion(this UrlHelper helper, string contentPath)
        {
            string version = System.Configuration.ConfigurationManager.AppSettings["HtmlFileImportVersion"];
            return string.Format("{0}?v={1}", helper.Content(contentPath), version);
        }

        public static string ToUrlStringParameter(this string parameter)
        {
            return parameter.Replace(" ","-").Replace(".","_");
        }

        public static string ToOriginalUrlStringParameter(this string parameter)
        {
            return parameter.Replace("-", " ").Replace("_",".");
        }

        public static string ToJSNumericValue(this object value)
        {
            return value.ToString().Replace(",",".");
        }

        public static SelectList ToSelectList<TEntity>(this IEnumerable<TEntity> collection,
            Func<TEntity, bool> selected = null, string valueMember = null, string displayMember = null) where TEntity : EntityBase
        {
            var list = collection.ToList();

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
            Func<TEntity, bool> selected = null) where TEntity : EntityBase
        {            
            var list = repository.Get(filter).OrderBy(p=>p.DescriptionName).ToList();
            return ToSelectList(list, selected);
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

        public static MultiSelectList ToMultiSelectList<TEntity>(this Repository<TEntity> repository,
            Expression<Func<TEntity, bool>> filter = null, IEnumerable selectedValues = null, 
            string valueMember = null, string displayMember=null) where TEntity : EntityBase
        {
            var list = repository.Get(filter).ToList();
            if (string.IsNullOrEmpty(valueMember))
                valueMember = "Key";
            if (string.IsNullOrEmpty(displayMember))
                displayMember = "DescriptionName";
            return new MultiSelectList(list,valueMember,displayMember, selectedValues);
        }

        public static SelectList ToSelectList(this Enum enumeration)
        {
            var source = Enum.GetValues(enumeration.GetType());

            var items = new Dictionary<int, string>();

            var displayAttributeType = typeof(DisplayAttribute);

            foreach (var value in source)
            {
                FieldInfo field = value.GetType().GetField(value.ToString());

                DisplayAttribute attrs = (DisplayAttribute)field.GetCustomAttributes(displayAttributeType, false).FirstOrDefault();

                if(attrs != null)
                    items.Add(Convert.ToInt32(value), attrs.GetName());
            }

           
            return new SelectList(items, "Key", "Value", enumeration);            
        }
    }
}

namespace System.Web.Mvc.Html
{
    public static class DropDownExtensions
    {
        public static MvcHtmlString DropDownTimeFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object htmlAttributes = null)
        {
            List<SelectListItem> items = new List<SelectListItem>();          

            DateTime StartDateTime = new DateTime(1900,1,1);
            for (int i = 1; i <= 48; i++)
            {
                items.Add(new SelectListItem()
                {
                    Value = StartDateTime.ToString(),
                    Text = StartDateTime.ToShortTimeString()
                });
                StartDateTime = StartDateTime.AddMinutes(30);
            }

            var result = htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            return result;
        }

        public static MvcHtmlString DropDownDurationFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object htmlAttributes = null, string selected = "30")
        {
            List<SelectListItem> items = new List<SelectListItem>();
            for (int min = 15; min <= 300; min += 5)
            {
                string display = string.Empty;
                if (min > 60)
                    display = string.Format("{0} {1} {2} min", min / 60, min / 60 > 1 ? "hrs" : "hr", min - (min / 60) * 60);
                else
                    display = string.Format("{0} min", min);
                items.Add(new SelectListItem() { Value = min.ToString(), Text = display, Selected = min.ToString() == selected });
            }            

            var result = htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            return result;
        }
        
        public static MvcHtmlString DropDownDayOfMonthListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object htmlAttributes = null, string selectedValue = "0")
        {
            List<SelectListItem> items = new List<SelectListItem>();
            
            items.Add(new SelectListItem()
            {
                Value = null,
                Text = ""
            });

            for (int i = 1; i <= 31;i++)
            {                
                items.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }            

            var result = htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            return result;
        }

        public static MvcHtmlString DropDownYearListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object htmlAttributes = null, string selectedValue = "0", string nullDisplayText = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem()
            {
                Value = null,
                Text = nullDisplayText
            });

            for (int year = Sic.Web.Mvc.Session.CurrentDateTime.Year;
                    year  >= Sic.Web.Mvc.Session.CurrentDateTime.Year - 110; year--)
            {
                items.Add(new SelectListItem()
                            {
                                Value = year.ToString(),
                                Text = year.ToString()       
                            });
            }
            
            var result = htmlHelper.DropDownListFor(expression, items, htmlAttributes);
            return result;
        }

        public static MvcHtmlString DropDownMonthListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel,
            TProperty>> expression, object htmlAttributes = null, string selectedValue = "0")
        {
            var newitems = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat
                .MonthNames                
                .Select((monthName, index) => new SelectListItem
                {
                    Value = (index + 1).ToString(),
                    Text = monthName,
                    Selected = (selectedValue == (index + 1).ToString())
                });

            var result = htmlHelper.DropDownListFor(expression, newitems, htmlAttributes);
            return result;
        }
    }

    public static class LinkExtensions
    {
        public static MvcHtmlString ActionImage(this HtmlHelper htmlHelper, string imgSrc,
            string alt, string actionName, string controllerName, object routeValues = null, 
            object htmlAttributes = null, object imgHtmlAttributes = null)
        {
            UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
            string imgtag = htmlHelper.ImageTag(imgSrc, alt, imgHtmlAttributes);
            string url = urlHelper.Action(actionName, controllerName, routeValues);

            TagBuilder imglink = new TagBuilder("a");
            imglink.MergeAttribute("href", url);
            imglink.InnerHtml = imgtag;
            imglink.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            return MvcHtmlString.Create(imglink.ToString());
        }

        public static MvcHtmlString Image(this HtmlHelper helper,
                             string url,
                             string altText,
                             object htmlAttributes)
        {
            string imglink = ImageTag(helper, url, altText, htmlAttributes);
            return MvcHtmlString.Create(imglink.ToString());
        }

        private static string ImageTag(this HtmlHelper helper,
                             string url,
                             string altText,
                             object htmlAttributes)
        {
            TagBuilder builder = new TagBuilder("img");
            builder.Attributes.Add("src", url);
            builder.Attributes.Add("alt", altText);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return builder.ToString(TagRenderMode.SelfClosing);
        }


    }
}
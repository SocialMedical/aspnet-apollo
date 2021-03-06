﻿using System.Web.Mvc;
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
using System.Web.Mvc.Html;

namespace Sic.Web.Mvc.Html
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
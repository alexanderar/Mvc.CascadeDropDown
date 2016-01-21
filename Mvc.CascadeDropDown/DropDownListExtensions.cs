﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Mvc.CascadeDropDown
{
    public static class DropDownListExtensions
    {
        //0 - triggerMemberInfo.Name
        //1 - url
        //2 - ajaxActionParamName
        //3 - optionLabel
        //4 - dropdownElementId
        private const string JqueryScriptFormat = @"<script>
$(function() {{
    $('#{0}').change(function () {{
        var value = $('#{0}').val();
        var items = '<option value="""">{3}</option>';
        if(value === """" || value == null)
        {{
            $('#{4}').html(items);  
            $('#{4}').val('');
            $('#{4}').change();
            return;
        }}              
        var url = ""{1}"";       
        $.getJSON(url + '?{2}=' + $('#{0}').val(), function (data) {{
            
            $.each(data, function (i, d) {{
                if (d.Selected) {{
                    items += ""<option value='"" + d.Value + "" ' selected>"" + d.Text + ""</option>"";
                }} else {{
                    items += ""<option value='"" + d.Value + ""'>"" + d.Text + ""</option>"";
                }}
            }});
            $('#{4}').html(items); 
        }});
    }});
}});
</script>";


        /// <summary>
        ///     The pure JavaScript  format.
        /// </summary>
        /// <remarks>
        ///     0 - triggerMemberInfo.Name
        ///     1 - URL
        ///     2 - ajaxActionParamName
        ///     3 - optionLabel
        ///     4 - dropdownElementId
        ///     5 - Preselected Value
        ///     6 - if element should be disabled when parent not selected, will contain setAttribute('disabled','disabled')
        ///     command
        ///     7 - if element was initially disabled, will contain removeAttribute('disabled') command
        /// </remarks>
        private const string PureJsScriptFormat = @"<script>       
    function initCascadeDropDownFor{4}() {{
        var triggerElement = document.getElementById('{0}');
        var targetElement = document.getElementById('{4}');
        var preselectedValue = '{5}';
        triggerElement.addEventListener('change', function(e) {{
            {7}
            var value = triggerElement.value;
            var items = '<option value="""">{3}</option>';            
            if (!value) {{
                targetElement.innerHTML = items;
                targetElement.value = '';                
                var event = document.createEvent('HTMLEvents');
                event.initEvent('change', true, false);
                targetElement.dispatchEvent(event);
                {6}
                return;
            }}
            var url = '{1}?{2}=' + value;
            var request = new XMLHttpRequest();
            request.open('GET', url, true);
            var isSelected = false;
            request.onload = function () {{
                if (request.status >= 200 && request.status < 400) {{
                    // Success!
                    var data = JSON.parse(request.responseText);
                    if (data) {{                        
                        data.forEach(function(item, i) {{                              
                            if (item.Selected == true) {{
                                items += '<option value=""' + item.Value + '"" selected>' + item.Text + '</option>';
                            }} else {{
                                items += '<option value=""' + item.Value + '"">' + item.Text + '</option>';
                            }}
                        }});
                        targetElement.innerHTML = items;  
                        if(preselectedValue)
                        {{                           
                            targetElement.value = preselectedValue;                            
                            preselectedValue = null;                           
                        }}  
                        var event = document.createEvent('HTMLEvents');
                        event.initEvent('change', true, false);
                        targetElement.dispatchEvent(event);                                                                                          
                    }}
                }} else {{
                    console.log(request.statusText);
                }}
            }};

            request.onerror = function (error) {{
                console.log(error);
            }};

            request.send();
        }});
        if(triggerElement.value && !targetElement.value)
        {{
            var event = document.createEvent('HTMLEvents');
            event.initEvent('change', true, false);
            triggerElement.dispatchEvent(event);           
        }} 
    }};

    if (document.readyState != 'loading') {{
        initCascadeDropDownFor{4}();
    }} else {{
        document.addEventListener('DOMContentLoaded', initCascadeDropDownFor{4});
    }}
</script>";


        public static MvcHtmlString CascadingDropDownList<TModel, TProperty>(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            Expression<Func<TModel, TProperty>> triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = "",
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null)
        {
            MemberInfo triggerMemberInfo = GetMemberInfo(triggeredByProperty);
            if (triggerMemberInfo == null)
            {
                throw new ArgumentException("triggeredByProperty argument is invalid");
            }

            return CascadingDropDownList(
                htmlHelper,
                inputName,
                inputId,
                triggerMemberInfo.Name,
                url,
                ajaxActionParamName,
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes);
        }

        public static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = "",
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null)
        {

            return CascadingDropDownList(
                htmlHelper,
                inputName,
                inputId,
                triggeredByProperty,
                url,
                ajaxActionParamName,
                GetPropStringValue(htmlHelper.ViewData.Model, inputName),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null
                ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                : new RouteValueDictionary());
        }

        private static string GetPropStringValue(object src, string propName)
        {
            string stringVal = null;
            if (src != null)
            {
                object propVal = src.GetType().GetProperty(propName).GetValue(src, null);
                stringVal = propVal != null ? propVal.ToString() : null;
            }
            return stringVal;
        }

        private static string GetPropStringValue<TModel, TProp>(TModel src, Expression<Func<TModel, TProp>> expression)
        {
            Func<TModel, TProp> func = expression.Compile();
            string selectedValString = string.Empty;
            if (src != null)
            {
                TProp propVal = func(src);
                string defaultValString = typeof (TProp).IsValueType && Nullable.GetUnderlyingType(typeof (TProp)) == null
                    ? Activator.CreateInstance(typeof (TProp)).ToString()
                    : string.Empty;
                if ((defaultValString != String.Empty && propVal.ToString() != defaultValString) ||
                    (defaultValString == String.Empty && propVal != null))
                {
                    selectedValString = propVal.ToString();
                }
            }
            return selectedValString;
        }

        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty, TProperty2>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            Expression<Func<TModel, TProperty2>> triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = "",
            bool disabledWhenParrentNotSelected = false,
            object htmlAttributes = null)
        {
            MemberInfo triggerMemberInfo = GetMemberInfo(triggeredByProperty);
            MemberInfo dropDownElement = GetMemberInfo(expression);

            if (dropDownElement == null)
            {
                throw new ArgumentException("expression argument is invalid");
            }

            if (dropDownElement == null)
            {
                throw new ArgumentException("triggeredByProperty argument is invalid");
            }

            string dropDownElementName = dropDownElement.Name;
            string dropDownElementId = GetDropDownElementId(htmlAttributes) ?? dropDownElement.Name;

            return CascadingDropDownList(htmlHelper,
                dropDownElementName,
                dropDownElementId,
                triggerMemberInfo.Name,
                url,
                ajaxActionParamName,
                GetPropStringValue(htmlHelper.ViewData.Model, expression),
                optionLabel,
                disabledWhenParrentNotSelected,
                htmlAttributes != null
                ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                : new RouteValueDictionary());
        }

        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string triggeredByPropertyWithId,
            string url,
            string ajaxActionParamName,
            string optionLabel = "",
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null)
        {
            MemberInfo dropDownElement = GetMemberInfo(expression);

            if (dropDownElement == null)
            {
                throw new ArgumentException("expression argument is invalid");
            }

            string dropDownElementName = dropDownElement.Name;
            string dropDownElementId = GetDropDownElementId(htmlAttributes) ?? dropDownElement.Name;

            return CascadingDropDownList(
                htmlHelper,
                dropDownElementName,
                dropDownElementId,
                triggeredByPropertyWithId,
                url,
                ajaxActionParamName,
                GetPropStringValue(htmlHelper.ViewData.Model, expression),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null
                ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                : new RouteValueDictionary());
        }

        private static string GetDropDownElementId(object htmlAttributes)
        {
            if (htmlAttributes != null)
            {
                PropertyInfo[] properties = htmlAttributes.GetType().GetProperties();
                PropertyInfo prop = properties.FirstOrDefault(p => p.Name.ToUpperInvariant() == "ID");
                if (prop != null)
                {
                    return prop.GetValue(htmlAttributes, null).ToString();
                }
            }
            return null;
        }

        public static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = "",
            bool disabledWhenParentNotSelected = false,
            RouteValueDictionary htmlAttributes = null)
        {
            return CascadingDropDownList(
               htmlHelper,
               inputName,
               inputId,
               triggeredByProperty,
               url,
               ajaxActionParamName,
               GetPropStringValue(htmlHelper.ViewData.Model, inputName),
               optionLabel,
               disabledWhenParentNotSelected,
               htmlAttributes);
        }

        private static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string selectedValue,
            string optionLabel = "",
            bool disabledWhenParentNotSelected = false,
            RouteValueDictionary htmlAttributes = null)
        {
            if (disabledWhenParentNotSelected)
            {
                if (htmlAttributes == null)
                {
                    htmlAttributes = new RouteValueDictionary();
                }

                htmlAttributes.Add("disabled", "disabled");
            }

            MvcHtmlString defaultDropDownHtml = htmlHelper.DropDownList(
                inputName,
                new List<SelectListItem>(),
                optionLabel,
                htmlAttributes);

            string script;

            if (disabledWhenParentNotSelected)
            {
                script = string.Format(
                    PureJsScriptFormat,
                    triggeredByProperty,
                    url,
                    ajaxActionParamName,
                    optionLabel,
                    inputId,
                    selectedValue,
                    "targetElement.setAttribute('disabled','disabled');",
                    "targetElement.removeAttribute('disabled');");
            }
            else
            {
                script = string.Format(
                    PureJsScriptFormat,
                    triggeredByProperty,
                    url,
                    ajaxActionParamName,
                    optionLabel,
                    inputId,
                    selectedValue,
                    string.Empty,
                    string.Empty);
            }

            string spanEventHandler = "<span id='" + inputId + "evenhHandler'></span>";

            string cascadingDropDownString = spanEventHandler + Environment.NewLine + defaultDropDownHtml +
                                             Environment.NewLine + script;

            return new MvcHtmlString(cascadingDropDownString);
        }


        /// <summary>
        ///     The get member info.
        /// </summary>
        /// <param inputName="propSelector">
        ///     The prop selector.
        /// </param>
        /// <typeparam inputName="TModel">
        /// </typeparam>
        /// <typeparam inputName="TProp">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="MemberInfo" />.
        /// </returns>
        private static MemberInfo GetMemberInfo<TModel, TProp>(Expression<Func<TModel, TProp>> propSelector)
        {
            var body = propSelector.Body as MemberExpression;
            return body != null ? body.Member : null;
        }
    }
}

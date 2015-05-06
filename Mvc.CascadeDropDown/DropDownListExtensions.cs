using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;


namespace Mvc.CascadeDropDown
{
    using System.Linq.Expressions;
    using System.Reflection;

    public static class DropDownListExtensions
    {
        //0 - triggerMemberInfo.Name
        //1 - url
        //2 - actionParam
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
                items += ""<option value='"" + d.Value + ""'>"" + d.Text + ""</option>"";
            }});
            $('#{4}').html(items);                
        }});
    }});
}});
</script>";

        
        //0 - triggerMemberInfo.Name
        //1 - url
        //2 - actionParam
        //3 - optionLabel
        //4 - dropdownElementId
        //5 - if element should be disabled when parrent not selected, will contain setAttribute('disabled','disabled') command
        //6 - if element was initially disabled, will contain removeAttribute('disabled') command 
        
        private const string PureJSScriptFormat = @"<script>       
    function initCascadeDropDownFor{4}() {{
        var triggerElement = document.getElementById('{0}');
        triggerElement.addEventListener('change', function(e) {{
            var value = triggerElement.value;
            var items = '<option value="""">{3}</option>';
            var targetElement = document.getElementById('{4}');
            if (value === '' || value == null) {{
                targetElement.innerHTML = items;
                targetElement.value = '';
                {5}
                var event = document.createEvent('HTMLEvents');
                event.initEvent('change', true, false);
                targetElement.dispatchEvent(event);
                return;
            }}
            var url = '{1}?{2}=' + value;
            var request = new XMLHttpRequest();
            request.open('GET', url, true);

            request.onload = function () {{
                if (request.status >= 200 && request.status < 400) {{
                    // Success!
                    var data = JSON.parse(request.responseText);
                    if (data) {{
                        data.forEach(function(item, i) {{
                            items += '<option value=""' + item.Value + '"">' + item.Text + '</option>';
                        }});
                        targetElement.innerHTML = items;
                        {6}
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
    }};

    if (document.readyState != 'loading') {{
        initCascadeDropDownFor{4}();
    }} else {{
        document.addEventListener('DOMContentLoaded', initCascadeDropDownFor{4});
    }}
</script>";

        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            Expression<Func<TModel, TProperty>> triggeredByProperty,
            string url,
            string actionParam,
            string optionLabel = "",
            bool disabledWhenParrentNotSelected = false,
            object htmlAttributes = null)
        {
            RouteValueDictionary dictionary = null;
            if (htmlAttributes != null)
            {
                dictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            }
            if (disabledWhenParrentNotSelected)
            {
                if (dictionary == null)
                {
                    dictionary = new RouteValueDictionary();
                }
                dictionary.Add("disabled", "disabled");
            }
            var defaultDropDownHtml = htmlHelper.DropDownListFor(
                expression,
                new List<SelectListItem>(),
                optionLabel,
                dictionary);

            var triggerMemberInfo = GetMemberInfo(triggeredByProperty);
            var dropDownElement = GetMemberInfo(expression);

            if (dropDownElement == null)
            {
                throw new ArgumentException("expression argument is invalid");
            }

            if (dropDownElement == null)
            {
                throw new ArgumentException("triggeredByProperty argument is invalid");
            }
            string script = string.Empty;

            if (disabledWhenParrentNotSelected)
            {
                script = string.Format(
                PureJSScriptFormat,
                triggerMemberInfo.Name,
                url,
                actionParam,
                optionLabel,
                dropDownElement.Name,
                "targetElement.setAttribute('disabled','disabled');",
                "targetElement.removeAttribute('disabled');");
            }
            else
            {
                script = string.Format(
                PureJSScriptFormat,
                triggerMemberInfo.Name,
                url,
                actionParam,
                optionLabel,
                dropDownElement.Name,
                string.Empty,
                string.Empty);
            }
            
            var cascadingDropDownString = defaultDropDownHtml + Environment.NewLine + script;

            return new MvcHtmlString(cascadingDropDownString);
        }

        /// <summary>
        /// The get member info.
        /// </summary>
        /// <param name="propSelector">
        /// The prop selector.
        /// </param>
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProp">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MemberInfo"/>.
        /// </returns>
        private static MemberInfo GetMemberInfo<TModel, TProp>(Expression<Func<TModel, TProp>> propSelector)
        {
            var body = propSelector.Body as MemberExpression;
            return body != null ? body.Member : null;
        }
    }
}
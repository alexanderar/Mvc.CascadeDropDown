using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Mvc.CascadeDropDown
{
    public static class DropDownListExtensions
    {
        /// <summary>
        /// 0 - cascading dropdown element Id
        /// 1 - triggeredByProperty - id of parent element that triggers data loading
        /// 2 - preselected value
        /// 3 - if element was initially disabled, will contain removeAttribute('disabled') command
        /// 4 - if optionLabel is set should be set to '<option value="""">optionLabel</option>' otherwise should be set to ""
        /// 5 - if element should be disabled when parent not selected, will contain setAttribute('disabled','disabled') command
        /// </summary>
        private const string Js1CreateInitFunction = @"function initCascadeDropDownFor{0}() {{
        var triggerElement = document.getElementById('{1}');
        var targetElement = document.getElementById('{0}');
        var preselectedValue = '{2}';
        triggerElement.addEventListener('change', function(e) {{
            {3}
            var value = triggerElement.value;
            var items = {4};            
            if (!value) {{
                targetElement.innerHTML = items;
                targetElement.value = '';                
                var event = document.createEvent('HTMLEvents');
                event.initEvent('change', true, false);
                targetElement.dispatchEvent(event);
                {5}
                return;
            }}";
        //var items = '<option value="""">{4}</option>'; 

        /// <summary>
        /// 2 in order CONDITIONAL
        /// {0} - CascadeDropDownOptions.BeforeSend function name
        /// {1} - ajaxActionParamName
        /// </summary>
        private const string Js2GenerateJsonToSendFromFunctionFormat = @"
var jsonToSend = {{ {1} : value }};
var updatedJson = {0}(jsonToSend);
if(updatedJson){{jsonToSend = updatedJson}}";

        /// <summary>
        /// 2 in order CONDITIONAL
        /// {0} - ajaxActionParamName
        /// </summary>
        private const string Js2SimpleGenerateJsonToSendFormat = @"var jsonToSend = {{ {0} : value }};";

        /// <summary>
        /// 3 in order CONDITIONAL
        /// used when CascadeDropDownOptions.HttpMethod is set to POST
        /// </summary>
        private const string Js3InitializePostRequest =
          @"var request = new XMLHttpRequest();            
            var url = targetElement.dataset.cascadeDdUrl;
            request.open('POST', url, true);
            request.setRequestHeader('Content-Type', 'application/json');";

        /// <summary>
        /// 3 in order CONDITIONAL
        /// used when CascadeDropDownOptions.HttpMethod is not set, or set to GET.
        /// </summary>
        private const string Js3InitializeGetRequest =
          @"var request = new XMLHttpRequest();            
            var url = targetElement.dataset.cascadeDdUrl;var appndSgn = url.indexOf('?') > -1 ? '&' : '?';
            var qs = Object.keys(jsonToSend).map(function(key){return key+'='+jsonToSend[key]}).join('&');
            request.open('GET', url+appndSgn+qs, true);";

        /// <summary>
        /// 4 in order
        /// {0} -  will have a call to CascadeDropDownOptions.OnCompleteGetData if it was set.
        /// {1} -  will have a call to CascadeDropDownOptions.OnSuccessGetData if it was set.
        /// </summary>
        private const string Js4OnLoadFormat =
          @"var isSelected = false;
            request.onload = function () {{                
                if (request.status >= 200 && request.status < 400) {{
                    var data = JSON.parse(request.responseText);                    
                    {0}
                    {1}
                    if (data) {{
                        data.forEach(function(item, i) {{
                            items += '<option value=""' + item.Value + '""'
                            if(item.Disabled){{items += ' disabled'}}
                            items += '>' + item.Text + '</option>';
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
                }}
            }};";

        /// <summary>
        /// 5 in order CONDITIONAL
        /// {0} -  will have a call to CascadeDropDownOptions.OnCompleteGetData if it was set.
        /// {1} -  will have a call to CascadeDropDownOptions.OnFailureGetData if it was set.
        /// </summary>
        private const string Js5ErrorCallback = 
            @"request.onerror = function (error) {{
                {0}{1}
            }};";


        /// <summary>
        /// 6 in order CONDITIONAL        
        /// </summary>
        private const string Js6SendPostRequest =
         @"request.send(JSON.stringify(jsonToSend));";

        /// <summary>
        /// 6 in order CONDITIONAL        
        /// </summary>
        private const string Js6SendGetRequest = @"request.send();";

        /// <summary>
        /// Last in order 
        /// {0} - cascading dropdown element Id
        /// </summary>
        private const string Js7EndFormat = @"
        }});
        if(triggerElement.value && !targetElement.value)
        {{
            var event = document.createEvent('HTMLEvents');
            event.initEvent('change', true, false);
            triggerElement.dispatchEvent(event);           
        }} 
    }};

    if (document.readyState != 'loading') {{
        initCascadeDropDownFor{0}();
    }} else {{
        document.addEventListener('DOMContentLoaded', initCascadeDropDownFor{0});
    }}";

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
        ///     8 - if modify data before send function is provided, will contain the code that invokes this function
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
            var jsonToSend = {{ {2} : value }};
            var request = new XMLHttpRequest();
            request.open('POST', '{1}', true);
            request.setRequestHeader('Content-Type', 'application/json');
            var isSelected = false;
            request.onload = function () {{
                if (request.status >= 200 && request.status < 400) {{
                    // Success!
                    var data = JSON.parse(request.responseText);
                    if (data) {{                        
                        data.forEach(function(item, i) {{                              
                            items += '<option value=""' + item.Value + '"">' + item.Text + '</option>';                                
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
           
            {8}                        
            request.send(JSON.stringify(jsonToSend));
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
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,           
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
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
                htmlAttributes,
                options);
        }

        public static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,           
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
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
                : new RouteValueDictionary(), 
                options);
        }

        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty, TProperty2>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            Expression<Func<TModel, TProperty2>> triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
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

            var dropDownElementName = dropDownElement.Name;
            var dropDownElementId = GetDropDownElementId(htmlAttributes) ?? dropDownElement.Name;

            return CascadingDropDownList(htmlHelper,
                dropDownElementName,
                dropDownElementId,
                triggerMemberInfo.Name,
                url,
                ajaxActionParamName,
                GetPropStringValue(htmlHelper.ViewData.Model, expression),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null
                ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                : new RouteValueDictionary(),
                options);
        }

        public static MvcHtmlString CascadingDropDownListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string triggeredByPropertyWithId,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
            var dropDownElement = GetMemberInfo(expression);

            if (dropDownElement == null)
            {
                throw new ArgumentException("expression argument is invalid");
            }

            var dropDownElementName = dropDownElement.Name;
            var dropDownElementId = GetDropDownElementId(htmlAttributes) ?? dropDownElement.Name;

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
                : new RouteValueDictionary(),
                options);
        }       

        public static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string inputId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            RouteValueDictionary htmlAttributes = null,
            CascadeDropDownOptions options = null)
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
               htmlAttributes,
               options);
        }

        private static MvcHtmlString CascadingDropDownList(
            this HtmlHelper htmlHelper,
            string inputName,
            string cascadeDdElementId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string selectedValue,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,          
            RouteValueDictionary htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new RouteValueDictionary();
            }

            htmlAttributes.Add("data-cascade-dd-url", url);
            var setDisableString = string.Empty;
            var removeDisabledString = string.Empty;

            if (disabledWhenParentNotSelected)
            {               
                htmlAttributes.Add("disabled", "disabled");
                setDisableString = "targetElement.setAttribute('disabled','disabled');";
                removeDisabledString = "targetElement.removeAttribute('disabled');";
            }

           
            var defaultDropDownHtml = htmlHelper.DropDownList(
                inputName,
                new List<SelectListItem>(),
                optionLabel,
                htmlAttributes);

            var scriptBuilder = new StringBuilder();
            var optionLblStr = optionLabel == null ? "''" : string.Format(@"'<option value="""">{0}</option>'", optionLabel);
            scriptBuilder.AppendFormat(Js1CreateInitFunction, cascadeDdElementId, triggeredByProperty, selectedValue, removeDisabledString, optionLblStr, setDisableString);
            ApplyJsonToSendString(ref scriptBuilder, ajaxActionParamName, options);
            ApplyRequestString(ref scriptBuilder, options);
            ApplyOnLoadString(ref scriptBuilder, options);
            ApplyErrorCallbackString(ref scriptBuilder, options);
            ApplySendRequestString(ref scriptBuilder, options);
            scriptBuilder.AppendFormat(Js7EndFormat, cascadeDdElementId);
            string script;
            if(options != null && options.EnableMinification)
            {
                var minifier = new WebMarkupMin.NUglify.NUglifyJsMinifier(new WebMarkupMin.NUglify.NUglifyJsMinificationSettings {
                    LocalRenaming = WebMarkupMin.NUglify.LocalRenaming.CrunchAll,
                    OutputMode = WebMarkupMin.NUglify.OutputMode.SingleLine
                });
                var minificationResult = minifier.Minify(scriptBuilder.ToString(), true);
                script = string.Concat("<script>", minificationResult.MinifiedContent, "</script>");
            }
            else
            {
                script = string.Concat("<script>", scriptBuilder.ToString(), "</script>");
            }
            return new MvcHtmlString(string.Concat(defaultDropDownHtml.ToString(),Environment.NewLine, script));
        }

        private static void ApplyJsonToSendString(ref StringBuilder builder, string ajaxParam, CascadeDropDownOptions options)
        {
            builder.Append( options == null || string.IsNullOrEmpty(options.BeforeSend) ?
                string.Format(Js2SimpleGenerateJsonToSendFormat, ajaxParam) :
                string.Format(Js2GenerateJsonToSendFromFunctionFormat, options.BeforeSend, ajaxParam));
        }

        private static void ApplyRequestString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            builder.Append(options == null || options.HttpMethod == null || !options.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ?
                Js3InitializeGetRequest : Js3InitializePostRequest);
        }      

        private static void ApplyOnLoadString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            var onComplete = string.Empty;
            var onSuccess = string.Empty;
            if(options!= null)
            {
                if(!string.IsNullOrEmpty(options.OnCompleteGetData))
                {
                    onComplete = string.Format("{0}(data, null);", options.OnCompleteGetData);
                }
                if (!string.IsNullOrEmpty(options.OnSuccessGetData))
                {
                    onSuccess = string.Format("{0}(data);", options.OnSuccessGetData);
                }
            }
            builder.AppendFormat(Js4OnLoadFormat, onComplete, onSuccess);
        }

        private static void ApplyErrorCallbackString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            var onComplete = string.Empty;
            var onFailure = string.Empty;
            if (options != null && (!string.IsNullOrEmpty(options.OnCompleteGetData) || !string.IsNullOrEmpty(options.OnFailureGetData)))
            {
                if (!string.IsNullOrEmpty(options.OnCompleteGetData))
                {
                    onComplete = string.Format("{0}(null, error);", options.OnCompleteGetData);
                }
                if (!string.IsNullOrEmpty(options.OnSuccessGetData))
                {
                    onFailure = string.Format("{0}(error);", options.OnFailureGetData);
                }
            }
            builder.AppendFormat(Js5ErrorCallback, onComplete, onFailure);
        }

        private static void ApplySendRequestString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            builder.Append(options == null || options.HttpMethod == null || !options.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ?
                Js6SendGetRequest : Js6SendPostRequest);
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
            var selectedValString = string.Empty;
            if (src != null)
            {
                TProp propVal = func(src);
                string defaultValString = typeof(TProp).IsValueType && Nullable.GetUnderlyingType(typeof(TProp)) == null
                    ? Activator.CreateInstance(typeof(TProp)).ToString()
                    : string.Empty;
                if ((defaultValString != string.Empty && propVal.ToString() != defaultValString) ||
                    (defaultValString == string.Empty && propVal != null))
                {
                    selectedValString = propVal.ToString();
                }
            }
            return selectedValString;
        }

        private static string GetDropDownElementId(object htmlAttributes)
        {
            if (htmlAttributes != null)
            {
                var properties = htmlAttributes.GetType().GetProperties();
                var prop = properties.FirstOrDefault(p => p.Name.ToUpperInvariant() == "ID");
                if (prop != null)
                {
                    return prop.GetValue(htmlAttributes, null).ToString();
                }
            }
            return null;
        }
    }
}
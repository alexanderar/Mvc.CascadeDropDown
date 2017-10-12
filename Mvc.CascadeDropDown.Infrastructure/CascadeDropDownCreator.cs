// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CascadeDropDownCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The cascade drop down creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///     The cascade drop down creator.
    /// </summary>
    public static class CascadeDropDownCreator
    {
        /// <summary>
        ///     The JS 1 create initialization function.
        /// </summary>
        private const string Js1CreateInitFunction = @"function initCascadeDropDownFor{0}() {{
        var triggerElement = document.getElementById('{1}');
        var targetElement = document.getElementById('{0}');
        var preselectedValue = '{2}';
        triggerElement.onchange = function(e) {{
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

        /// <summary>
        ///     2 in order CONDITIONAL
        ///     {0} - CascadeDropDownOptions.BeforeSend function name
        ///     {1} - ajaxActionParamName
        /// </summary>
        private const string Js2GenerateJsonToSendFromFunctionFormat = @"
            var jsonToSend = {{ {1} : value }};
            var updatedJson = {0}(jsonToSend);
            if(updatedJson){{jsonToSend = updatedJson}}
            ";

        /// <summary>
        ///     2 in order CONDITIONAL
        ///     {0} - ajaxActionParamName
        /// </summary>
        private const string Js2SimpleGenerateJsonToSendFormat = @"var jsonToSend = {{ {0} : value }};";

        /// <summary>
        ///     3 in order CONDITIONAL
        ///     used when CascadeDropDownOptions.HttpMethod is not set, or set to GET.
        /// </summary>
        private const string Js3InitializeGetRequest = @"var request = new XMLHttpRequest();            
            var url = targetElement.dataset.cascadeDdUrl;var appndSgn = url.indexOf('?') > -1 ? '&' : '?';
            var qs = Object.keys(jsonToSend).map(function(key){return key+'='+jsonToSend[key]}).join('&');
            request.open('GET', url+appndSgn+qs, true);";

        /// <summary>
        ///     3 in order CONDITIONAL
        ///     used when CascadeDropDownOptions.HttpMethod is set to POST
        /// </summary>
        private const string Js3InitializePostRequest = @"var request = new XMLHttpRequest();            
            var url = targetElement.dataset.cascadeDdUrl;
            request.open('POST', url, true);
            request.setRequestHeader('Content-Type', 'application/json');";

#if (NETSTANDARD1_6) 
         private const string Js4OnLoadFormatOptionsCreateScript = @"items += '<option value=""' + item.value + '""'
                            if(item.disabled){{items += ' disabled'}}
                            items += '>' + item.text + '</option>';";
#else
        private const string Js4OnLoadFormatOptionsCreateScript = @"items += '<option value=""' + item.Value + '""'
                            if(item.Disabled){{items += ' disabled'}}
                            items += '>' + item.Text + '</option>';";
#endif


        /// <summary>
        ///     4 in order
        ///     {0} -  will have a call to CascadeDropDownOptions.OnCompleteGetData if it was set.
        ///     {1} -  will have a call to CascadeDropDownOptions.OnSuccessGetData if it was set.
        ///     {2} -  will have a call to CascadeDropDownOptions.OnFailureGetData if it was set.
        /// </summary>
        private const string Js4OnLoadFormat = @"var isSelected = false;
            request.onload = function () {{
                if (request.status >= 200 && request.status < 400) {{
                    var data = JSON.parse(request.responseText);
                    {0}
                    {1}
                    if (data) {{
                        data.forEach(function(item, i) {{" +
                        Js4OnLoadFormatOptionsCreateScript +
                        @"}});
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
                if (request.status >= 400){{
                    var placeholder = targetElement.dataset.optionLbl;
                    targetElement.innerHTML = null;
                    if(placeholder)
                    {{
                        targetElement.innerHTML = '<option value="""">' + placeholder + '</option>';
                    }}
                    {2}
                }}
            }};";



        /// <summary>
        ///     5 in order CONDITIONAL
        ///     {0} -  will have a call to CascadeDropDownOptions.OnCompleteGetData if it was set.
        ///     {1} -  will have a call to CascadeDropDownOptions.OnFailureGetData if it was set.
        /// </summary>
        private const string Js5ErrorCallback = @"request.onerror = function () {{
                {0}{1}
            }};";

        /// <summary>
        ///     6 in order CONDITIONAL
        /// </summary>
        private const string Js6SendGetRequest = @"request.send();";

        /// <summary>
        ///     6 in order CONDITIONAL
        /// </summary>
        private const string Js6SendPostRequest = @"request.send(JSON.stringify(jsonToSend));";

        /// <summary>
        ///     Last in order
        ///     {0} - cascading dropdown element Id
        /// </summary>
        private const string Js7EndFormat = @"
        }};
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
        /// The create.
        /// </summary>
        /// <param name="defaultDropDownFactory">
        /// The default drop down factory.
        /// </param>
        /// <param name="inputName">
        /// The input name.
        /// </param>
        /// <param name="cascadeDdElementId">
        /// The cascade dd element id.
        /// </param>
        /// <param name="triggeredByProperty">
        /// The triggered by property.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="ajaxActionParamName">
        /// The ajax action param name.
        /// </param>
        /// <param name="selectedValue">
        /// The selected value.
        /// </param>
        /// <param name="optionLabel">
        /// The option label.
        /// </param>
        /// <param name="disabledWhenParentNotSelected">
        /// The disabled when parent not selected.
        /// </param>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Create(
            Func<string, IDictionary<string, object>, string, string> defaultDropDownFactory,
            string inputName,
            string cascadeDdElementId,
            string triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string selectedValue,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            IDictionary<string, object> htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }

            htmlAttributes.Add("data-cascade-dd-url", url);
            var setDisableString = string.Empty;
            var removeDisabledString = string.Empty;

            if (optionLabel != null)
            {
                htmlAttributes.Add("data-option-lbl", optionLabel);
            }

            if (disabledWhenParentNotSelected)
            {
                htmlAttributes.Add("disabled", "disabled");
                setDisableString = "targetElement.setAttribute('disabled','disabled');";
                removeDisabledString = "targetElement.removeAttribute('disabled');";
            }

            var defaultDropDownHtml = defaultDropDownFactory(inputName, htmlAttributes, optionLabel);

            var scriptBuilder = new StringBuilder();
            var optionLblStr = optionLabel == null ? "''" : $@"'<option value="""">{optionLabel}</option>'";
            ApplyCreateInitFunction(ref scriptBuilder, cascadeDdElementId, triggeredByProperty, selectedValue, removeDisabledString, optionLblStr, setDisableString);
            ApplyJsonToSendString(ref scriptBuilder, ajaxActionParamName, options);
            ApplyRequestString(ref scriptBuilder, options);
            ApplyOnLoadString(ref scriptBuilder, options);
            ApplyErrorCallbackString(ref scriptBuilder, options);
            ApplySendRequestString(ref scriptBuilder, options);
            ApplyEndString(ref scriptBuilder, cascadeDdElementId);
            var script = string.Concat("<script>", scriptBuilder.ToString(), "</script>");
            return string.Concat(defaultDropDownHtml, Environment.NewLine, script);
        }

        /// <summary>
        /// The apply create init function.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="cascadeDdElementId">
        /// The cascade dd element id.
        /// </param>
        /// <param name="triggeredByProperty">
        /// The triggered by property.
        /// </param>
        /// <param name="selectedValue">
        /// The selected value.
        /// </param>
        /// <param name="removeDisabledString">
        /// The remove disabled string.
        /// </param>
        /// <param name="optionLblStr">
        /// The option lbl str.
        /// </param>
        /// <param name="setDisableString">
        /// The set disable string.
        /// </param>
        private static void ApplyCreateInitFunction(
            ref StringBuilder builder,
            string cascadeDdElementId,
            string triggeredByProperty,
            string selectedValue,
            string removeDisabledString,
            string optionLblStr,
            string setDisableString)
        {
            builder.AppendFormat(Js1CreateInitFunction, cascadeDdElementId, triggeredByProperty, selectedValue, removeDisabledString, optionLblStr, setDisableString);
        }

        /// <summary>
        /// The apply end string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="cascadeDdElementId">
        /// The cascade drop down element element id.
        /// </param>
        private static void ApplyEndString(ref StringBuilder builder, string cascadeDdElementId)
        {
            builder.AppendFormat(Js7EndFormat, cascadeDdElementId);
        }

        /// <summary>
        /// The apply error callback string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void ApplyErrorCallbackString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            var onComplete = string.Empty;
            var onFailure = string.Empty;
            if (options != null && (!string.IsNullOrEmpty(options.OnCompleteGetData) || !string.IsNullOrEmpty(options.OnFailureGetData)))
            {
                if (!string.IsNullOrEmpty(options.OnCompleteGetData))
                {
                    onComplete = string.Format("{0}(null, request.responseText);", options.OnCompleteGetData);
                }

                if (!string.IsNullOrEmpty(options.OnSuccessGetData))
                {
                    onFailure = string.Format("{0}(request.responseText, request.status, request.statusText);", options.OnFailureGetData);
                }

                builder.AppendFormat(Js5ErrorCallback, onComplete, onFailure);
            }
        }

        /// <summary>
        /// The apply json to send string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="ajaxParam">
        /// The ajax param.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void ApplyJsonToSendString(ref StringBuilder builder, string ajaxParam, CascadeDropDownOptions options)
        {
            builder.Append(
                string.IsNullOrEmpty(options?.BeforeSend)
                    ? string.Format(Js2SimpleGenerateJsonToSendFormat, ajaxParam)
                    : string.Format(Js2GenerateJsonToSendFromFunctionFormat, options.BeforeSend, ajaxParam));
        }

        /// <summary>
        /// The apply on load string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void ApplyOnLoadString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            var onComplete = string.Empty;
            var onSuccess = string.Empty;
            var onFailure = string.Empty;
            if (options != null)
            {
                if (!string.IsNullOrEmpty(options.OnCompleteGetData))
                {
                    onComplete = string.Format("{0}(data, null);", options.OnCompleteGetData);
                }

                if (!string.IsNullOrEmpty(options.OnSuccessGetData))
                {
                    onSuccess = string.Format("{0}(data);", options.OnSuccessGetData);
                }

                if (!string.IsNullOrEmpty(options.OnFailureGetData))
                {
                    onFailure = string.Format("{0}(request.responseText, request.status, request.statusText);", options.OnFailureGetData);
                }
            }

            builder.AppendFormat(Js4OnLoadFormat, onComplete, onSuccess, onFailure);
        }

        /// <summary>
        /// The apply request string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void ApplyRequestString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            builder.Append(
                options?.HttpMethod == null || !options.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase)
                    ? Js3InitializeGetRequest
                    : Js3InitializePostRequest);
        }

        /// <summary>
        /// The apply send request string.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        private static void ApplySendRequestString(ref StringBuilder builder, CascadeDropDownOptions options)
        {
            builder.Append(
                options?.HttpMethod == null || !options.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ? Js6SendGetRequest : Js6SendPostRequest);
        }
    }
}
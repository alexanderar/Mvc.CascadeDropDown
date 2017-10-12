﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropDownListExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The drop down list extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Text.Encodings.Web;

    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
    using Microsoft.AspNetCore.Routing;

    using Mvc.CascadeDropDown.Infrastructure;

    /// <summary>
    ///     The drop down list extensions.
    /// </summary>
    public static class DropDownListExtensions
    {
        /// <summary>
        /// The cascading drop down list.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="inputName">
        /// The input name.
        /// </param>
        /// <param name="inputId">
        /// The input id.
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
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IHtmlContent"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static IHtmlContent CascadingDropDownList<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
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
            var triggeredByPropId = htmlHelper.GetElementIdFromExpression(triggeredByProperty);
            if (string.IsNullOrEmpty(triggeredByPropId))
            {
                throw new ArgumentException("triggeredByProperty argument is invalid");
            }

            return CascadingDropDownList(
                htmlHelper,
                inputName,
                inputId,
                triggeredByPropId,
                url,
                ajaxActionParamName,
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes,
                options);
        }

        /// <summary>
        /// The cascading drop down list.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="inputName">
        /// The input name.
        /// </param>
        /// <param name="inputId">
        /// The input id.
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
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static IHtmlContent CascadingDropDownList(
            this IHtmlHelper htmlHelper,
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
                Utils.GetPropStringValue(htmlHelper.ViewData.Model, inputName),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : new RouteValueDictionary(),
                options);
        }

        /// <summary>
        /// The cascading drop down list.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="inputName">
        /// The input name.
        /// </param>
        /// <param name="inputId">
        /// The input id.
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
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static IHtmlContent CascadingDropDownList(
            this IHtmlHelper htmlHelper,
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
                Utils.GetPropStringValue(htmlHelper.ViewData.Model, inputName),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes,
                options);
        }

        /// <summary>
        /// The cascading drop down list for.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
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
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <typeparam name="TProperty2">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static IHtmlContent CascadingDropDownListFor<TModel, TProperty, TProperty2>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            Expression<Func<TModel, TProperty2>> triggeredByProperty,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
            var dropDownElementName = htmlHelper.GetElementNameFromExpression(expression);
            var dropDownElementId = Utils.GetDropDownElementId(htmlAttributes) ?? htmlHelper.GetElementIdFromExpression(expression);

            if (string.IsNullOrEmpty(dropDownElementName) || string.IsNullOrEmpty(dropDownElementId))
            {
                throw new ArgumentException("expression argument is invalid");
            }

            var triggeredByPropId = htmlHelper.GetElementIdFromExpression(triggeredByProperty);
            if (string.IsNullOrEmpty(triggeredByPropId))
            {
                throw new ArgumentException("triggeredByProperty argument is invalid");
            }

            return CascadingDropDownList(
                htmlHelper,
                dropDownElementName,
                dropDownElementId,
                triggeredByPropId,
                url,
                ajaxActionParamName,
                Utils.GetPropStringValue(htmlHelper.ViewData.Model, expression),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : new RouteValueDictionary(),
                options);
        }

        /// <summary>
        /// The cascading drop down list for.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <param name="triggeredByPropertyWithId">
        /// The triggered by property with id.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="ajaxActionParamName">
        /// The ajax action param name.
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
        /// <typeparam name="TModel">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IHtmlContent{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static IHtmlContent CascadingDropDownListFor<TModel, TProperty>(
            this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            string triggeredByPropertyWithId,
            string url,
            string ajaxActionParamName,
            string optionLabel = null,
            bool disabledWhenParentNotSelected = false,
            object htmlAttributes = null,
            CascadeDropDownOptions options = null)
        {
            var dropDownElementName = htmlHelper.GetElementNameFromExpression(expression);
            var dropDownElementId = Utils.GetDropDownElementId(htmlAttributes) ?? htmlHelper.GetElementIdFromExpression(expression);

            if (string.IsNullOrEmpty(dropDownElementName) || string.IsNullOrEmpty(dropDownElementId))
            {
                throw new ArgumentException("expression argument is invalid");
            }

            return CascadingDropDownList(
                htmlHelper,
                dropDownElementName,
                dropDownElementId,
                triggeredByPropertyWithId,
                url,
                ajaxActionParamName,
                Utils.GetPropStringValue(htmlHelper.ViewData.Model, expression),
                optionLabel,
                disabledWhenParentNotSelected,
                htmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : new RouteValueDictionary(),
                options);
        }

        /// <summary>
        /// The cascading drop down list.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
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
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        private static IHtmlContent CascadingDropDownList(
            this IHtmlHelper htmlHelper,
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
            Func<string, IDictionary<string, object>, string, string> defaultDropDownFactory = (inptName, htmlAttrsDictionnary, optLbl) =>
                {
                    using (var writer = new StringWriter())
                    {
                        htmlHelper.DropDownList(inptName, new List<SelectListItem>(), optLbl, htmlAttrsDictionnary).WriteTo(writer, HtmlEncoder.Default);
                        return writer.ToString();
                    }
                };

            return new HtmlString(
                CascadeDropDownCreator.Create(
                    defaultDropDownFactory,
                    inputName,
                    cascadeDdElementId,
                    triggeredByProperty,
                    url,
                    ajaxActionParamName,
                    selectedValue,
                    optionLabel,
                    disabledWhenParentNotSelected,
                    htmlAttributes,
                    options));
        }

        /// <summary>
        /// The get element id from expression.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// The type of model
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of property
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetElementIdFromExpression<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var fullName = htmlHelper.GetElementNameFromExpression(expression);
            return htmlHelper.GenerateIdFromName(fullName);
        }

        /// <summary>
        /// The get element name from expression.
        /// </summary>
        /// <param name="htmlHelper">
        /// The html helper.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// The type of model
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of property
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetElementNameFromExpression<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
    }
}
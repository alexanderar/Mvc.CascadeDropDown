// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="">
//   
// </copyright>
// <summary>
//   The utils.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.Infrastructure
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     The utils.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The object activator.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="T">
        /// The type of the object to create
        /// </typeparam>
        /// <returns>The object of type T</returns>
        private delegate T ObjectActivator<T>(params object[] args);

        /// <summary>
        /// The get activator.
        /// </summary>
        /// <param name="constructor">
        /// The constructor.
        /// </param>
        /// <typeparam name="T">
        /// Type of object to create
        /// </typeparam>
        /// <returns>
        /// The <see cref="ObjectActivator{T}"/>.
        /// </returns>
        private static ObjectActivator<T> GetActivator<T>(ConstructorInfo constructor)
        {
            var paramsInfo = constructor.GetParameters();

            // create a single param of type object[]
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[paramsInfo.Length];

            // pick each arg from the params array 
            // and create a typed expression of them
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // make a NewExpression that calls the
            // ctor with the args we just created
            var newExp = Expression.New(constructor, argsExp);

            // create a lambda with the New
            // Expression as body and our param object[] as arg
            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            // compile it
            var compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled;
        }

        /// <summary>
        /// The get prop string value.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="TModel">
        /// The type model
        /// </typeparam>
        /// <typeparam name="TProp">
        /// The type of the property of the model
        /// </typeparam>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPropStringValue<TModel, TProp>(TModel source, Expression<Func<TModel, TProp>> expression)
        {
            var func = expression.Compile();
            var selectedValString = string.Empty;
            if (source != null)
            {
                var propVal = func(source);
                var defaultValString = typeof(TProp).IsValueType() && Nullable.GetUnderlyingType(typeof(TProp)) == null
#if NET40
                                           ? GetActivator<TProp>(typeof(TProp).GetConstructors().First()).ToString()
#else
                                           ? GetActivator<TProp>(typeof(TProp).GetTypeInfo().GetConstructors().First()).ToString()
#endif
                                           : string.Empty;
                if (defaultValString != string.Empty && propVal.ToString() != defaultValString || defaultValString == string.Empty && propVal != null)
                {
                    selectedValString = propVal.ToString();
                }
            }

            return selectedValString;
        }

        /// <summary>
        /// The get drop down element id.
        /// </summary>
        /// <param name="htmlAttributes">
        /// The html attributes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDropDownElementId(object htmlAttributes)
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

        /// <summary>
        /// The get member info.
        /// </summary>
        /// <param inputName="propSelector">
        /// The prop selector.
        /// </param>
        /// <param name="propSelector">
        /// The prop Selector.
        /// </param>
        /// <typeparam inputName="TModel">
        /// </typeparam>
        /// <typeparam inputName="TProp">
        /// </typeparam>
        /// <returns>
        /// The <see cref="MemberInfo"/>.
        /// </returns>
        public static MemberInfo GetMemberInfo<TModel, TProp>(Expression<Func<TModel, TProp>> propSelector)
        {
            var body = propSelector.Body as MemberExpression;
            return body?.Member;
        }

        /// <summary>
        /// The get prop string value.
        /// </summary>
        /// <param name="src">
        /// The source.
        /// </param>
        /// <param name="propName">
        /// The prop name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPropStringValue(object source, string propName)
        {
            string stringVal = null;
            if (source != null)
            {
#if NET40
                var propVal = source.GetType().GetProperty(propName).GetValue(source, null);
#else
                var propVal = source.GetType().GetTypeInfo().GetProperty(propName).GetValue(source, null);
#endif
                stringVal = propVal?.ToString();
            }

            return stringVal;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The type extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Mvc.CascadeDropDown.Infrastructure
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The type extensions.
    /// </summary>
    public static class TypeExtensions
    {

#if NET40

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

                /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="PropertyInfo[]"/>.
        /// </returns>
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetProperties();
        }
    
#else

        /// <summary>
        /// The is value type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="PropertyInfo[]"/>.
        /// </returns>
        public static PropertyInfo[] GetProperties(this Type type)
        {
            return type.GetTypeInfo().GetProperties();
        }

#endif
    }
}
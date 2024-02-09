using System;
using System.Reflection;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    public static class CoreUtils
    {
        #region Getters

        /// <summary>
        ///  Get value of a Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<U, V>(string memberName, U containingInstance)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            return (V)typeof(U).GetField(memberName, flags).GetValue(containingInstance);
        }

        /// <summary>
        /// Get value of a static Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<U, V>(string memberName)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            return (V)typeof(U).GetField(memberName, flags).GetValue(null);
        } 

        /// <summary>
        ///  Get value of a Member on static Containing Type via reflection
        /// </summary>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<V>(string memberName, Type containingType)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
            return (V)containingType.GetField(memberName, flags).GetValue(null);
        }

        #endregion

        #region Setters

        /// <summary>
        ///  Get value of a Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static void SetMember<U, V>(string memberName, U containingInstance, V value, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            typeof(U).GetField(memberName, flags).SetValue(containingInstance, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get Invoke method on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Return Type</typeparam>
        public static V InvokeMethod<U, V>(string methodName, U containingInstance, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            return (V)typeof(U).GetMethod(methodName, flags).Invoke(containingInstance, parameters);
        }

        /// <summary>
        /// Get Invoke method on Containing Type via reflection with no return
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        public static void InvokeMethod<U>(string methodName, U containingInstance, params object[] parameters)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            typeof(U).GetMethod(methodName, flags).Invoke(containingInstance, parameters);
        }

        #endregion

        #region Type

        /// <summary>
        /// Get Type of enum
        /// </summary>
        /// <param name="enumQualifiedPath"></param>
        /// <param name="containingAssembly"></param>
        /// <returns></returns>
        public static Type GetEnum(string enumQualifiedPath, Assembly containingAssembly)
        {
            return containingAssembly.GetType(enumQualifiedPath);
        }

        /// <summary>
        /// Get value of an enum by reflection.
        /// Note: containingInstance and enumContainingAssembly can be of different Types
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        /// <param name="enumQualifiedPath">Fully qualified name of Type of enum</param>
        /// <param name="enumContainingAssembly">Assembly containing the enum at enumQualifiedPath</param>
        public static V GetEnumValue<U, V>(string memberName, U containingInstance, string enumQualifiedPath, Assembly enumContainingAssembly)
        {
            var enumType = GetEnum(enumQualifiedPath, enumContainingAssembly);
            var enumValue = GetMember<U, object>(memberName, containingInstance);

            return (V)Enum.ToObject(enumType, enumValue);
        }

        public static void SetEnumValue<U, V>(string memberName, U containingInstance, V value)
        {
            SetMember(memberName, containingInstance, Enum.Parse(typeof(V), value.ToString()));
        }

        #endregion
    }
}

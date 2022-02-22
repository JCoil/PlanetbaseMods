using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using ModData = UnityModManagerNet.UnityModManager.ModEntry;

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
        ///  Get underlying value of an enum 
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static int GetEnumValue<V>(string memberName, Type containingType, V enumType) where V:Type
        {
            var value = GetMember <V> (memberName, containingType);
            return (int) Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(V)));
        }

        #endregion
    }
}

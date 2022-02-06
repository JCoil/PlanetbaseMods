using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using ModData = UnityModManagerNet.UnityModManager.ModEntry;

namespace PlanetbaseModUtilities
{
    public static class CoreUtils
    {
        /// <summary>
        ///  Get value of a Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static void SetMember<U, V>(string memberName, U containingInstance, V value, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            typeof(U).GetField(memberName, flags).SetValue(containingInstance, value);
        }


        /// <summary>
        /// Get value of a static Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<U, V>(string memberName, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static)
        {
            return (V)typeof(U).GetField(memberName, flags).GetValue(null);
        }

        /// <summary>
        ///  Get value of a Member on Containing Type via reflection
        /// </summary>
        /// <typeparam name="U">Containing Type</typeparam>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<U,V>(string memberName, U containingInstance, BindingFlags flags = BindingFlags.NonPublic|BindingFlags.Instance)
        {
            return (V)typeof(U).GetField(memberName, flags).GetValue(containingInstance);
        }

        /// <summary>
        ///  Get value of a Member on static Containing Type via reflection
        /// </summary>
        /// <typeparam name="V">Member Type</typeparam>
        public static V GetMember<V>(string memberName, Type containingType, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static)
        {
            return (V)containingType.GetField(memberName, flags).GetValue(null);
        }
    }
}

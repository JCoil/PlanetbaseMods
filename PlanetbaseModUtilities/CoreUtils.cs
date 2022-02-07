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
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        public static V GetMember<U,V>(string memberName, U containingObject, BindingFlags flags = BindingFlags.NonPublic|BindingFlags.Instance)
        {
            var type = typeof(U);
            Debug.Log("Got Type");
            var field = type.GetField(memberName, flags);
            Debug.Log("Got Field");
            var value = field.GetValue(containingObject);
            Debug.Log("Got Value");

            return (V)value;
        }


        /// <summary>
        ///  Get value of a Member on static Containing Type via reflection
        /// </summary>
        /// <typeparam name="U"></typeparam>
        public static V GetMember<V>(string memberName, Type containingType, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static)
        {
            return (V)containingType.GetField(memberName, flags).GetValue(null);
        }
    }
}

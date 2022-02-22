using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;

namespace PlanetbaseModUtilities
{
    public static class TypeListUtils
    {
        /// <summary>
        /// Add an entry into a Global TypeList store, eg. TypeList<ComponentType, ComponentTypeList>
        /// Man this is a confusing structure, but this allows us to register new values with the protected method TypeList<T, LT>.add() 
        /// </summary>
        /// <param name="typeList">New TypeList entry</param>
        /// <param name="item">The TypeList to add to in the form TypeList<ComponentType, ComponentTypeList> equivalent to just ComponentTypeList</param>
        public static void AddType<T, LT>(this TypeList<T, LT> typeList, T item) where LT : TypeList<T, LT>, new()
        {
            CoreUtils.InvokeMethod("add", typeList, item);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Planetbase;

namespace PlanetbaseModUtilities
{
    public static class TypeListUtils
    {
        public static void AddType<T, LT>(this TypeList<T, LT> typeList, T item) where LT : TypeList<T, LT>, new()
        {
            CoreUtils.InvokeMethod("add", typeList, item);
        }
    }
}

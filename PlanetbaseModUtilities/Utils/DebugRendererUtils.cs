using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities.Utils
{
    /// <summary>
    /// Helper methods and members for DebugRenderer
    /// </summary>
    public static class DebugRendererUtils
    {
        private static Type debugRendererType;
        private static Type DebugRendererType
        {
            get
            {
                if(debugRendererType == null)
                {
                    debugRendererType= typeof(GameManager).Assembly.GetTypes().First(x => x.Name.ToUpper().Contains("DEBUGRENDERER"));
                }
                return debugRendererType;
            }
        }

        public static GameObject AddGroup(string group)
        {
            return (GameObject)DebugRendererType.GetMethod("addGroup", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { group });
        }

        public static void ClearGroup(string group)
        {
            DebugRendererType.GetMethod("clearGroup", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { group });
        }

        public static void AddLine(string group, Vector3 startPoint, Vector3 endPoint, Color color, float size)
        {
            DebugRendererType.GetMethod("addLine", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { group, startPoint, endPoint, color, size });
        }

        public static void AddCube(string group, Vector3 position, Color color, float size)
        {
            DebugRendererType.GetMethod("addCube", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { group, position, color, size });
        }
    }
}

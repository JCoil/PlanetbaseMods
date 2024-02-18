using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;
using System.Linq;

namespace ImprovedManufacturingLimits.Patches
{
    [HarmonyPatch(typeof(ManufactureLimits), nameof(ManufactureLimits.deserialize))]
    public class ManufactureLimitsDeserializePatch
    {
        public static void Postfix(XmlNode node)
        {
            if (node != null)
            {
                // Find any resource limits that are in mResourceLimits, but didn't have an xml entry
                // This isn't ideal, but it should only be required when a save is loaded that didn't previously have this mod installed
                foreach (var entry in ManufactureLimitsHelper.ResourceLimits)
                {
                    var resourceType = entry.Key;
                    var limit = entry.Value;
                    var nodeName = resourceType.GetType().Name + "-limit";

                    if (node[nodeName] == null)
                    {
                        limit.set(int.MaxValue);
                    }
                }
            }
        }
    }
}

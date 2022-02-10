using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic; 
using System.Text; 
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	/// <summary>
	/// Inject manufacture limit check before isDamaged to show as 'not damaged' if over limit
	/// </summary>
	[HarmonyPatch(typeof(ConstructionComponent), nameof(ConstructionComponent.isDamaged))]
	public class ConstructionComponentPatch
	{
		public static bool Prefix(ref ConstructionComponent __instance, ref bool __result, Specialization specialization)
		{
			if (!ManufactureLimitsHelper.IsUnderLimit(__instance))
			{
				__result = false; // New isDamaged returns false;
				return false; // Do not execute the rest of the method
			}

			return true; // Continue executing the method
		}
	}
}

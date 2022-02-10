using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	/// <summary>
	/// Inject manufacture limit check after findMin to return null if ore over limit
	/// </summary>
	[HarmonyPatch(typeof(Module), nameof(Module.findMine))]
	public class ModulePatch
	{
		/// <summary>
		/// Retrieve the original return module value and pass through unless over limit 
		/// </summary>
		public static Module Postfix(Module module)
        {			
			return ManufactureLimitsHelper.IsUnderLimit<Ore>() ? module : null;
        }
	}
}

using Planetbase;
using PlanetbaseModUtilities;
using Redirection;
using System;
using System.Collections.Generic; 
using System.Text; 
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	public class GuiAmountSelectorImproved : GuiAmountSelector
	{
		public GuiAmountSelectorImproved(int min, int max, RefInt value, GuiDefinitions.Callback changeCallback, int flags, Texture2D icon)
			: base(min, max, 1, value, changeCallback, flags)
		{
			mIcon = icon;
		}

		public Texture2D mIcon;
	}

	public static class SelectorHelper
	{ 
		public static void OnPlusImproved(GuiAmountSelector selector)
		{
			var current = CoreUtils.GetMember<GuiAmountSelector, RefInt>("mCurrent", selector);
			var changeCallback = CoreUtils.GetMember<GuiAmountSelector, GuiDefinitions.Callback>("mChangeCallback", selector);

			var max = CoreUtils.GetMember<GuiAmountSelector, int>("mMax", selector);
			var min = CoreUtils.GetMember<GuiAmountSelector, int>("mMin", selector);
			var step = CoreUtils.GetMember<GuiAmountSelector, int>("mStep", selector);

			if (current.get() < max)
			{
				var keyMultiplier = (Event.current.control ? 100 : 1) * (Event.current.shift ? 10 : 1);
				var change = Math.Max(step, keyMultiplier);

				current.set(current.get() + change);

				// Check we're still below max

				if (current.get() > max)
                {
					current.set(max);
                }

				if (changeCallback != null)
				{
					changeCallback(selector);
				}
			}			
			else if (current.get() == 2147483647 && selector.hasFlag(GuiAmountSelector.FlagLoop))  
			{
				current.set(min);
			}
			else if (selector.hasFlag(GuiAmountSelector.FlagHasInfinity))
			{
				current.set(int.MaxValue);
				if (changeCallback != null)
				{
					changeCallback(selector);
				}
			}
		}

		public static void OnMinusImproved(GuiAmountSelector selector)
		{
			var current = CoreUtils.GetMember<GuiAmountSelector, RefInt>("mCurrent", selector);
			var changeCallback = CoreUtils.GetMember<GuiAmountSelector, GuiDefinitions.Callback>("mChangeCallback", selector);

			var max = CoreUtils.GetMember<GuiAmountSelector, int>("mMax", selector);
			var min = CoreUtils.GetMember<GuiAmountSelector, int>("mMin", selector);
			var step = CoreUtils.GetMember<GuiAmountSelector, int>("mStep", selector);

			if (current.get() == 2147483647)
			{
				current.set(max);
				if (changeCallback != null)
				{
					changeCallback(selector);
				}
			}			
			else if (current.get() > min)
			{
				var keyMultiplier = (Event.current.control ? 100 : 1) * (Event.current.shift ? 10 : 1);
				var change = Math.Max(step, keyMultiplier);

				current.set(current.get() - change);

				if (current.get() < min)
				{
					current.set(min);
				}

				if (changeCallback != null)
				{
					changeCallback(selector);
				}
			}			
			else if (current.get() == min && selector.hasFlag(GuiAmountSelector.FlagLoop))
			{
				current.set(int.MaxValue);
			}
		}
	}
}

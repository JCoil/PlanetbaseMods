using Planetbase;
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
			if (selector.mCurrent.get() < selector.mMax)
			{
				var keyMultiplier = (Event.current.control ? 100 : 1) * (Event.current.shift ? 10 : 1);
				var change = Math.Max(selector.mStep, keyMultiplier);

				selector.mCurrent.set(selector.mCurrent.get() + change);

				// Check we're still below max

				if (selector.mCurrent.get() > selector.mMax)
                {
					selector.mCurrent.set(selector.mMax);
                }

				if (selector.mChangeCallback != null)
				{
					selector.mChangeCallback(selector);
				}
			}			
			else if (selector.mCurrent.get() == 2147483647 && selector.hasFlag(GuiAmountSelector.FlagLoop))  
			{
				selector.mCurrent.set(selector.mMin);
			}
			else if (selector.hasFlag(GuiAmountSelector.FlagHasInfinity))
			{
				selector.mCurrent.set(int.MaxValue);
				if (selector.mChangeCallback != null)
				{
					selector.mChangeCallback(selector);
				}
			}
		}

		public static void OnMinusImproved(GuiAmountSelector selector)
		{
			if (selector.mCurrent.get() == 2147483647)
			{
				selector.mCurrent.set(selector.mMax);
				if (selector.mChangeCallback != null)
				{
					selector.mChangeCallback(selector);
				}
			}			
			else if (selector.mCurrent.get() > selector.mMin)
			{
				var keyMultiplier = (Event.current.control ? 100 : 1) * (Event.current.shift ? 10 : 1);
				var change = Math.Max(selector.mStep, keyMultiplier);

				selector.mCurrent.set(selector.mCurrent.get() - change);

				if (selector.mCurrent.get() < selector.mMin)
				{
					selector.mCurrent.set(selector.mMin);
				}

				if (selector.mChangeCallback != null)
				{
					selector.mChangeCallback(selector);
				}
			}			
			else if (selector.mCurrent.get() == selector.mMin && selector.hasFlag(GuiAmountSelector.FlagLoop))
			{
				selector.mCurrent.set(int.MaxValue);
			}
		}
	}
}

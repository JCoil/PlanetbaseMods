using HarmonyLib;
using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic; 
using System.Text; 
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	[HarmonyPatch(typeof(GuiRenderer), "renderAmountSelector")]
	public class GuiRendererPatch
	{
		/// <summary>
		/// New renderer to display icon amount selector if set
		/// </summary>
		public static bool Prefix(ref GuiRenderer __instance, GuiWindow window, GuiAmountSelector selector, float x, float y)
        {
			GUI.enabled = selector.isEnabled();
			float height = selector.getHeight();
			float num = height * 0.25f;

			var tooltip = Environment.NewLine + "Shift for 10" + Environment.NewLine + "Ctrl for 100";


			float labelExtraWidth = Math.Max(1, CoreUtils.GetMember<GuiAmountSelector, int>("mMax", selector).ToString().Length - 2) * num; // Pretty hacky way to get the ui spaced nicely

			if (selector is GuiAmountSelectorImproved selectorImproved && selectorImproved.mIcon != null)
			{
				__instance.renderIcon(x, y, GuiStyles.IconSizeSmall, selectorImproved.mIcon, selectorImproved.getTooltip());
				x += height + num / 2f;
			}

			if (__instance.renderButton(new Rect(x, y, height, height), new GUIContent("-", "Click to decrease" + tooltip), null))
			{
				SelectorHelper.OnMinusImproved(selector);		
			}

			x += height + num/2f;

			GUI.Label(new Rect(x, y, height * 1.75f, height), new GUIContent(selector.getText(), selector.getTooltip()), __instance.getLabelStyle(FontSize.Normal, FontStyle.Bold, TextAnchor.MiddleLeft, FontType.Normal));
			
			x += height + labelExtraWidth;

			if (selector.hasFlag(1)) // Extra small for percent
			{
				x += height * 0.5f;
			}

			if (__instance.renderButton(new Rect(x, y, height, height), new GUIContent("+", "Click to increase" + tooltip), null))
			{
				SelectorHelper.OnPlusImproved(selector);
			}

			GUI.enabled = true;

			return false; // Replace original method
		}
	}
}

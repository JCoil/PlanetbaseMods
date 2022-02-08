using Planetbase;
using PlanetbaseModUtilities;
using Redirection;
using System;
using System.Collections.Generic; 
using System.Text; 
using UnityEngine;

namespace ImprovedManufacturingLimits
{
    public class GuiRendererRedirect : GuiRenderer
	{
		/// <summary>
		/// New renderer to display iconon amaount selector if set
		/// </summary>
		[RedirectFrom(typeof(GuiRenderer))]
		public new void renderAmountSelector(GuiWindow window, GuiAmountSelector selector, float x, float y)
		{
			GUI.enabled = selector.isEnabled();
			float height = selector.getHeight();
			float num = height * 0.25f;

			var tooltip = Environment.NewLine + "Shift for 10" + Environment.NewLine + "Ctrl for 100";


			float labelExtraWidth = Math.Max(1, CoreUtils.GetMember<GuiAmountSelector, int>("mMax", selector).ToString().Length - 2) * num; // Pretty hacky way to get the ui spaced nicely

			if (selector is GuiAmountSelectorImproved selectorImproved && selectorImproved.mIcon != null)
			{
				this.renderIcon(x, y, GuiStyles.IconSizeSmall, selectorImproved.mIcon, selectorImproved.getTooltip());
				x += height + num / 2f;
			}

			if (this.renderButton(new Rect(x, y, height, height), new GUIContent("-", "Click to decrease" + tooltip), null))
			{
				SelectorHelper.OnMinusImproved(selector);		
			}

			x += height + num/2f;

			GUI.Label(new Rect(x, y, height * 1.75f, height), new GUIContent(selector.getText(), selector.getTooltip()), this.getLabelStyle(FontSize.Normal, FontStyle.Bold, TextAnchor.MiddleLeft, FontType.Normal));
			
			x += height + labelExtraWidth;

			if (selector.hasFlag(1)) // Extra small for percent
			{
				x += height * 0.5f;
			}

			if (this.renderButton(new Rect(x, y, height, height), new GUIContent("+", "Click to increase" + tooltip), null))
			{
				SelectorHelper.OnPlusImproved(selector);
			}

			GUI.enabled = true;
		}
	}
}

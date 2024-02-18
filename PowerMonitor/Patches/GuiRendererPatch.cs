using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PowerMonitor
{
	[HarmonyPatch(typeof(GuiRenderer), nameof(GuiRenderer.renderResourcePanel))]
	public class RenderResourcePanelPatch
    {
		public static void Postfix(ref GuiRenderer __instance, GuiResourcePanel resourcePanel)
		{
			Grid grid = Grid.getLargest();
			if (grid == null)
			{
				grid = new Grid();
			}

			var powerIndicator = new GuiIndicatorItem(grid.getPowerStorageIndicator());

			// Annoyingly we need to recalculate the position of the Resource panel

			float width = Screen.height * 0.315f;
			float height = powerIndicator.calculateTotalHeight();
			float x = Screen.width - width - GuiStyles.getIconMargin() * 0.5f;
			float y = GuiStyles.getIconMargin() * 0.5f + resourcePanel.getYOffset() + height;

			GUIStyle style = new GUIStyle(__instance.getLabelStyle(FontSize.Normal, FontStyle.Bold, TextAnchor.UpperLeft, FontType.Normal));

			var rect = new Rect(x, y, width, height);
			__instance.renderIndicator(rect, powerIndicator.getIndicator(), style);
		}

		public void renderResourcePanel(GuiResourcePanel resourcePanel)
		{
			float num = (float)GuiStyles.getIconMargin() * 0.75f;
			float num2 = (float)GuiStyles.getIconSizeSmall();
			ResourceAmounts totalAmounts = Resource.getTotalAmounts();
			int columns = resourcePanel.getColumns();
			float num3 = (float)Screen.height * 0.315f * (float)columns / 4f;
			float num4 = (float)Screen.width - num3 - (float)GuiStyles.getIconMargin() * 0.5f;
			float num5 = (float)GuiStyles.getIconMargin() * 0.5f + resourcePanel.getYOffset();
			int num6 = (totalAmounts.getCount() - 1) / columns + 1;
			float num7 = num2 + num;
			float height = (float)num6 * num7 + num * 2f;
			float num8 = num2 * 0.5f;
			float num9 = num8 * 0.5f;
			float num10 = (num3 - num * 2.5f) / (float)columns;
			this.renderBigBox(new Rect(num4, num5, num3, height), true, resourcePanel.getYOffset() != 0f, false);
			num4 += num * 1.75f;
			num5 += num * 1.5f;
			GUIStyle labelStyle = this.getLabelStyle(FontSize.Normal, FontStyle.Bold, TextAnchor.LowerLeft, FontType.Normal);
			float num11 = num4;
			float num12 = num5;
			int num13 = 0;
			Rect position = new Rect(0f, 0f, num10, num2);
			Rect position2 = new Rect(0f, 0f, num8, num8);
			int resourceInfoCount = resourcePanel.getResourceInfoCount();
			for (int i = 0; i < resourceInfoCount; i++)
			{
				ResourceInfo resourceInfo = resourcePanel.getResourceInfo(i);
				Texture2D texture2D = null;
				if (resourceInfo.getResourceType().hasFlag(128) && resourceInfo.getAmount() != resourceInfo.getFreeAmount())
				{
					if (resourceInfo.getFreeAmount() * 2 < resourceInfo.getAmount())
					{
						texture2D = ResourceList.StaticIcons.ManyUsed;
					}
					else if (resourceInfo.getFreeAmount() < resourceInfo.getAmount())
					{
						texture2D = ResourceList.StaticIcons.SomeUsed;
					}
				}
				GUIContent content = resourceInfo.getContent();
				position.x = num11;
				position.y = num12;
				GUI.DrawTexture(new Rect(position.x, position.y, position.height, position.height), ResourceList.getInstance().Ui.Icon);
				GUI.Label(position, content, labelStyle);
				if (texture2D != null)
				{
					position2.x = position.x + num2 - num9;
					position2.y = position.y - num9;
					GUI.DrawTexture(position2, texture2D);
				}
				num13++;
				if (num13 == columns)
				{
					num11 = num4;
					num12 += num7;
					num13 = 0;
				}
				else
				{
					num11 += num10;
				}
			}
		}
}

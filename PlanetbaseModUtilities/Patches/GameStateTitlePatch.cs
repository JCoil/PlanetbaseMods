using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    [HarmonyPatch(typeof(GameStateTitle), "onGui")]
    public class GameStateTitlePatch
    {
        public static void Postfix(ref GuiRenderer ___mGuiRenderer, ref float ___mRightOffset)
        {
            var menuButtonSize = GuiRenderer.getMenuButtonSize(FontSize.Huge);
            var num8 = menuButtonSize.y * 0.75f;
            var rect = new Rect(24 - ___mRightOffset, 24, num8, num8);

            if (___mGuiRenderer.renderButton(rect, new GUIContent(null, ResourceList.getInstance().Icons.Bot, "Mod Settings"), null))
            {
                GameManager.getInstance().setGameStateCredits();
            }
        }
    }
}

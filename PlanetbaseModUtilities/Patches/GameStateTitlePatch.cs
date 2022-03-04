using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PlanetbaseModUtilities
{
    [HarmonyPatch(typeof(TitleScene), "onGui")]
    public class GameStateTitlePatch
    {
        public static void Postfix(ref GuiRenderer ___mGuiRenderer, ref float ___mLeftOffset)
        {
            var buttonSize = GuiStyles.getIconSizeMedium();
            var rect = new Rect(24f - ___mLeftOffset, 24f, buttonSize, buttonSize);

            if (___mGuiRenderer.renderButton(rect, new GUIContent(null, ResourceList.getInstance().Icons.Bot, "Mod Settings"), null))
            {
                GameManager.getInstance().setNewState(new GameStateModSettings());
            }
        }
    }
}

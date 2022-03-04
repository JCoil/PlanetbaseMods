using Planetbase;
using PlanetbaseModUtilities;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace PlanetbaseModUtilities
{
	public class GameStateModSettings : GameState
	{
		private float mRightOffset;
		private Vector2 mScrollPosition;

		private readonly GuiRenderer mGuiRenderer = new GuiRenderer();

		readonly Texture2D BackgroundTexture;

		readonly float panelMargin;
		readonly float panelWidth;
        readonly float backgroundHeight;
        readonly float backgroundWidth;

		Rect scrollOuterRect;
		Rect scrollViewRect;
		Rect titleRect;
		Rect backgroundRect;

		public GameStateModSettings()
		{
			mRightOffset = Screen.width * 0.25f;
			mScrollPosition = new Vector2(0f, 0f);

			float panelPosY = Screen.height * 0.21f;

			panelMargin = Screen.height * 0.05f;
			panelWidth = panelMargin * 14f;

			scrollOuterRect = new Rect(0, panelPosY, panelWidth * 1.05f, (panelMargin * 2f + GuiStyles.getIconMargin()) * 5f);
			scrollViewRect = new Rect(0, panelPosY, panelWidth, (panelMargin + GuiStyles.getIconMargin()) * 50);
			titleRect = new Rect(0, Screen.height * 0.06f, panelWidth, Screen.height * 0.2f);

			BackgroundTexture = ResourceList.getInstance().Title.BackgroundLoadRight;
			backgroundHeight = Screen.height * BackgroundTexture.height / 1080f;
			backgroundWidth = backgroundHeight * BackgroundTexture.width / BackgroundTexture.height;

			backgroundRect = new Rect(0, (Screen.height - backgroundHeight) * 0.5f, backgroundWidth, backgroundHeight);
		}

		public override void onGui()
		{
			// Update X positions for panel swipe
			float panelPosX = Screen.width * 0.96f - panelWidth + mRightOffset;

			scrollOuterRect.x = panelPosX;
			scrollViewRect.x = panelPosX;
			titleRect.x = panelPosX;
			backgroundRect.x = Screen.width - backgroundWidth + mRightOffset;

			// Draw Title
			GUIStyle labelStyle = mGuiRenderer.getLabelStyle(FontSize.Huge, FontStyle.Bold, TextAnchor.MiddleCenter, FontType.Title);
			GUI.Label(titleRect, "Mod Settings", labelStyle);

			// Background
			GUI.DrawTexture(backgroundRect, BackgroundTexture);

			// Scrollview
			mScrollPosition = GUI.BeginScrollView(scrollOuterRect, mScrollPosition, scrollViewRect);
			var scrollEntryStep = GuiStyles.getIconMargin() + panelMargin;

			var scrollItemPos = scrollOuterRect.y + scrollEntryStep;

			var mod = FindMod("CheatTools");
			Debug.Log($"Displaying mod: {mod.Info.AssemblyName}");
			if (scrollItemPos + panelMargin > scrollOuterRect.y + mScrollPosition.y && scrollItemPos < scrollOuterRect.y + scrollOuterRect.height + mScrollPosition.y)
			{
				var modLabelStyle = mGuiRenderer.getLabelStyle(FontSize.Normal, FontStyle.Bold, TextAnchor.MiddleLeft, FontType.Normal);
				GUI.Label(new Rect(scrollOuterRect.x, scrollOuterRect.y, GuiStyles.getIconMargin(), scrollOuterRect.height), mod.Info.AssemblyName, modLabelStyle);
			}

			scrollItemPos += scrollEntryStep;


			GUI.EndScrollView();

			// Back Button
			if (mGuiRenderer.renderBackButton(new Vector2(panelPosX + panelWidth * 0.5f, Screen.height * 0.82f)))
			{
				Singleton<Profile>.getInstance().save();
				GameManager.getInstance().setGameStateTitle();
			}
		}

		public override bool isTitleState()
		{
			return true;
		}

		public override bool shouldDrawAnnouncement()
		{
			return false;
		}

		public override void update(float timeStep)
		{
			base.update(timeStep);
			Singleton<TitleScene>.getInstance().updateOffset(ref mRightOffset);
		}

		public override void destroy()
		{
			mGuiRenderer.destroy();
		}
	}
}

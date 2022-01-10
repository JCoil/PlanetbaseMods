using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CheatTools
{
    public class CheatTools : IMod
    {
        GameStateGame Game;
        GuiMenuSystem MenuSystem;
        GameGui GameGui;

        public GuiCheatMenuItem CheatMenuItem { get; private set; }

        public void Init()
        {
            StringList.mStrings.Add("cheat_menu", "Cheat Tools");
            StringList.mStrings.Add("tooltip_cheat_menu", "Cheat Tools");
            StringList.mStrings.Add("force_build", "Force Build");

            foreach(var entry in StringList.mStrings)
            {
                Debug.Log($"{entry.Key}:{entry.Value}");
            }

            Debug.Log("[MOD] Cheat Tools activated");
        }

        public void Update()
        {
            Game = GameManager.getInstance().getGameState() as GameStateGame;
            MenuSystem = Game.mMenuSystem as GuiMenuSystem;
            GameGui = Game.mGameGui as GameGui;

            if (MenuSystem != null && GameGui != null)
            {
                if (MenuSystem.mMenuMain != null && !MenuSystem.mMenuMain.mItems.Contains(CheatMenuItem))
                {
                    CheatMenuItem = new GuiCheatMenuItem(new GuiMenu(StringList.get("cheat_menu")), new GuiDefinitions.Callback(OnCheatMenuOpen));

                    MenuSystem.mMenuMain.addItem(CheatMenuItem);
                }
            }
        }

        private void OnCheatMenuOpen(object parameter)
        {
            if (GameGui.getWindow() is GuiCheatMenu)
            {
                GameGui.setWindow(null);
            }
            else
            {
                GuiCheatMenu guiCheatMenu = new GuiCheatMenu();
                GameGui.setWindow(guiCheatMenu);
            }
        }
    }

    public class GuiCheatMenuItem : GuiMenuItem
    {
        public GuiCheatMenuItem(GuiMenu menu, GuiDefinitions.Callback callback) : base(
             TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStorage>().getIcon(),
             StringList.get("tooltip_cheat_menu"),
             callback, menu, FlagMenuSwitch)
        {

        }
    }

    public class GuiCheatMenu : GuiWindow
    {
        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, null, 0, FontSize.Normal), null, null)
        {
            AddButton("force_build", new GuiDefinitions.Callback(OnForceBuild), true);
        }

        public void AddButton(string key, GuiDefinitions.Callback callback, bool enabled)
        {
            GuiButtonItem guiButtonItem = new GuiButtonItem(StringList.get(key), callback, FontType.Title);
            guiButtonItem.setEnabled(enabled);
            mRootItem.addChild(guiButtonItem);
        }

        private void OnForceBuild(object parameter)
        {
            try
            {
                if (Construction.mConstructions is List<Construction> constructions)
                {
                    foreach (var construction in constructions)
                    {
                        if (!construction.isBuilt())
                        {
                            Debug.Log("Forcing build on " + construction.getId());
                            construction.onBuilt();
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {

            }
        }
    }
}

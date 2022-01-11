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

        public void Init()
        {
            StringList.mStrings.Add("cheat_menu", "Cheat Tools");
            StringList.mStrings.Add("tooltip_cheat_menu", "Cheat Tools");

            StringList.mStrings.Add("force_build", "Force Build");
            StringList.mStrings.Add("unlock_tech", "Unlock all Tech");

            Debug.Log("[MOD] Cheat Tools activated");
        }

        public void Update()
        {
            Game = GameManager.getInstance().getGameState() as GameStateGame;

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.F11))
            {
                if (!(Game.mGameGui.getWindow() is GuiCheatMenu))
                {
                    GuiCheatMenu guiCheatMenu = new GuiCheatMenu();
                    Game.mGameGui.setWindow(guiCheatMenu);
                }
            }
        }
    }

    public class GuiCheatMenu : GuiWindow
    {
        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, null, 0, FontSize.Normal), null, null)
        {
            AddButton("force_build", new GuiDefinitions.Callback(OnForceBuild), true);
            AddButton("unlock_tech", new GuiDefinitions.Callback(OnUnlockTech), true);
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

        private void OnUnlockTech(object parameter)
        {
            if(TechManager.getInstance() is TechManager manager && TypeList<Tech, TechList>.getInstance().mTypeList is List<Tech> techList)
            {
                foreach(var tech in techList)
                {
                    if(!manager.isAcquired(tech))
                    {
                        manager.acquire(tech);
                    }
                }
            }
        }
    }
}

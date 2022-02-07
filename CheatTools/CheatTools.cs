using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager; 
using System.Reflection;
using PlanetbaseModUtilities;

namespace CheatTools
{
    public class CheatTools : ModBase
    {
        public static void Init(ModEntry modEntry) => InitializeMod(new CheatTools(), modEntry, "CheatTools");

<<<<<<< Updated upstream
            StringList.mStrings.Add("force_structures", "Force Structures");
            StringList.mStrings.Add("force_components", "Force Components");
            StringList.mStrings.Add("unlock_tech", "Unlock all Tech");

            Debug.Log("[MOD] Cheat Tools activated");
        }
=======
        static bool KeysDown = false;
>>>>>>> Stashed changes

        public override void OnInitialized()
        {
            StringUtils.GetGlobalStrings().Add("cheat_menu", "Cheat Tools");
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
            if (GameManager.getInstance().getGameState() is GameStateGame Game)
            {
                if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.F11))
                {
                    if (!KeysDown)
                    {
                        Game.toggleWindow<GuiCheatMenu>();
                        KeysDown = true;
                    }
                }
                else
                {
                    KeysDown = false;
                }
            }
        }
    }

    public class GuiCheatMenu : GuiWindow
    {
<<<<<<< Updated upstream
        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, null, 0, FontSize.Normal), null, null)
        {
            AddButton("force_structures", new GuiDefinitions.Callback(OnForceStructures), true);
            AddButton("force_components", new GuiDefinitions.Callback(OnForceComponents), true);
            AddButton("unlock_tech", new GuiDefinitions.Callback(OnUnlockTech), true);
=======
        Construction SelectedConstruction { get; set; }

        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, CheatToolsStrings.cheat_menu_tooltip, 0, FontSize.Normal), null, null)
        {
            if(Selection.getSelectedConstruction() is Construction construction && construction.getComponentCount() > 0)
            {
                SelectedConstruction = construction;
            }

            AddButton(CheatToolsStrings.force_structures, new GuiDefinitions.Callback(OnForceStructures), true);
            AddButton(CheatToolsStrings.force_components, new GuiDefinitions.Callback(OnForceComponents), true);
            AddButton(CheatToolsStrings.unlock_tech, new GuiDefinitions.Callback(OnUnlockTech), true);
            AddButton(CheatToolsStrings.clear_components, new GuiDefinitions.Callback(OnClearComponents), SelectedConstruction != null);
>>>>>>> Stashed changes
        }

        public void AddButton(string name, GuiDefinitions.Callback callback, bool enabled)
        {
<<<<<<< Updated upstream
            GuiButtonItem guiButtonItem = new GuiButtonItem(StringList.get(key), callback, FontType.Title);
=======
            GuiButtonItem guiButtonItem = new GuiButtonItem(name, callback, FontType.Normal);
>>>>>>> Stashed changes
            guiButtonItem.setEnabled(enabled);
            mRootItem.addChild(guiButtonItem);
        }

        private void OnForceStructures(object parameter)
        {
            if (BuildableUtils.GetConstructions() is List<Construction> constructions)
            {
                foreach (var construction in constructions)
                {
                    if (!construction.isBuilt())
                    {
                        construction.onBuilt();
                    }
                }
            }
        }

        private void OnForceComponents(object parameter)
        {
            try
            {
                if (BuildableUtils.GetComponents() is List<ConstructionComponent> components)
                {
                    foreach (var construction in components)
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
            if (TechManager.getInstance() is TechManager manager && TypeList<Tech, TechList>.get() is List<Tech> techList)
            {
                foreach (var tech in techList)
                {
                    if (!manager.isAcquired(tech))
                    {
                        manager.acquire(tech);
                    }
                }
            }
        }
    }
}

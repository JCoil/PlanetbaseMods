using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityModManagerNet.UnityModManager; 
using System.Reflection;
using PlanetbaseModUtilities;
using CheatTools.Patches;
using PlanetbaseModUtilities.Utils;

namespace CheatTools
{
    public class CheatTools : ModBase
    {
        public static new void Init(ModEntry modEntry) => InitializeMod(new CheatTools(), modEntry, "CheatTools");

        static bool KeysDown = false;

        public static bool ModuleAnarchyOn { get; set; } = false;

        public override void OnInitialized(ModEntry modEntry)
        {
            StringUtils.RegisterString("cheat_menu", "Cheat Tools");
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
        Construction SelectedConstruction { get; set; }

        GuiButtonItem moduleAnarchyButton;

        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, CheatToolsStrings.cheat_menu_tooltip, 0, FontSize.Normal), null, null)
        {
            if (Selection.getSelectedConstruction() is Construction construction && construction.getComponentCount() > 0)
            {
                SelectedConstruction = construction;
            }

            AddButton(CheatToolsStrings.force_structures, new GuiDefinitions.Callback(OnForceStructures), true);
            AddButton(CheatToolsStrings.force_components, new GuiDefinitions.Callback(OnForceComponents), true);
            AddButton(CheatToolsStrings.unlock_tech, new GuiDefinitions.Callback(OnUnlockTech), true);
            AddButton(CheatToolsStrings.clear_components, new GuiDefinitions.Callback(OnClearComponents), SelectedConstruction != null);

            moduleAnarchyButton = AddButton(CheatToolsStrings.GetModuleAnarchyText, new GuiDefinitions.Callback(OnModuleAnarchy), true);
        }

        public GuiButtonItem AddButton(string name, GuiDefinitions.Callback callback, bool enabled)
        {
            var guiButtonItem = new GuiButtonItem(name, callback, FontType.Normal);
            guiButtonItem.setEnabled(enabled);
            mRootItem.addChild(guiButtonItem);
            return guiButtonItem;
        }

        private void OnForceStructures(object parameter)
        {
            if (BuildableUtils.GetAllConstructions() is List<Construction> constructions)
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
                if (BuildableUtils.GetAllComponents() is List<ConstructionComponent> components)
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

        private void OnClearComponents(object parameter)
        {
            if (SelectedConstruction != null)
            {
                var constructions = new ConstructionComponent[SelectedConstruction.getComponentCount()];
                SelectedConstruction.getComponents().CopyTo(constructions);

                foreach(var component in constructions)
                {
                    component.destroy();
                }
            }
        }

        private void OnModuleAnarchy(object parameter)
        {
            CheatTools.ModuleAnarchyOn = !CheatTools.ModuleAnarchyOn;
            moduleAnarchyButton.SetText(CheatToolsStrings.GetModuleAnarchyText);
        }
    }
}

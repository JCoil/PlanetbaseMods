using ModWrapper;
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
    public class CheatTools : ModBase, IMod
    {
        public override void Init()
        {
            StringList.mStrings.Add("cheat_menu", "Cheat Tools");
            StringList.mStrings.Add("tooltip_cheat_menu", "Cheat Tools");

            StringList.mStrings.Add("force_structures", "Force Structures");
            StringList.mStrings.Add("force_components", "Force Components");
            StringList.mStrings.Add("unlock_tech", "Unlock all Tech");
            StringList.mStrings.Add("clear_components", "Clear Components");

            Debug.Log("[MOD] Cheat Tools activated");
        }

        public override void OnGameStart()
        {
            // Nothing required here
        }

        public override void Update(float timeStep)
        {
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
        Construction SelectedConstruction { get; set; }

        public GuiCheatMenu() : base(new GuiLabelItem(StringList.get("cheat_menu"), null, null, 0, FontSize.Normal), null, null)
        {
            if(Selection.getSelectedConstruction() is Construction construction && construction.getComponentCount() > 0)
            {
                SelectedConstruction = construction;
            }

            AddButton("force_structures", new GuiDefinitions.Callback(OnForceStructures), true);
            AddButton("force_components", new GuiDefinitions.Callback(OnForceComponents), true);
            AddButton("unlock_tech", new GuiDefinitions.Callback(OnUnlockTech), true);
            AddButton("clear_components", new GuiDefinitions.Callback(OnClearComponents), SelectedConstruction != null);
        }

        public void AddButton(string key, GuiDefinitions.Callback callback, bool enabled)
        {
            GuiButtonItem guiButtonItem = new GuiButtonItem(StringList.get(key), callback, FontType.Normal);
            guiButtonItem.setEnabled(enabled);
            mRootItem.addChild(guiButtonItem);
        }

        private void OnForceStructures(object parameter)
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

        private void OnForceComponents(object parameter)
        {
            try
            {
                if (ConstructionComponent.mComponents is List<ConstructionComponent> components)
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
            if (TechManager.getInstance() is TechManager manager && TypeList<Tech, TechList>.getInstance().mTypeList is List<Tech> techList)
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
    }
}

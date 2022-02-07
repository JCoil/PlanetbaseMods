using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace StorageGuru
{
    public static class MenuController
    {
        public static GuiMenuSystem MenuSystem;

        private static GuiStorageMenu StorageMenu;
        private static GuiStorageMenuItem StorageMenuItem;

        public static void Init(GuiMenuSystem menuSystem)
        {
            MenuSystem = menuSystem;
            StorageMenu = new GuiStorageMenu();
            StorageMenuItem = new GuiStorageMenuItem(StorageMenu);
        }

        public static void Update(GuiMenuSystem menuSystem)
        {
            //this is mostly to remove the "remove unused parameter if its not part of a shipped public API" message
            if (menuSystem is null)
            {
                throw new ArgumentNullException(nameof(menuSystem));
            }

            FieldInfo currentMenuGet = typeof(GuiMenuSystem).GetField("mCurrentMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            var currentMenuSet = currentMenuGet.GetValue(currentMenuGet) as GuiMenu;
            if (currentMenuSet == StorageMenu)
            {
                StorageMenu.Update();
            }

            // If we're viewing an action menu
            FieldInfo menuActionGet = typeof(GuiMenuSystem).GetField("mMenuAction", BindingFlags.NonPublic | BindingFlags.Instance);
            var menuActionSet = menuActionGet.GetValue(menuActionGet) as GuiMenu;
            if (menuActionSet is GuiMenu actionMenu)
            {
                // That is for a built storage module
                if (Selection.getSelected() is Planetbase.Module module && module.getModuleType() is ModuleTypeStorage && module.isBuilt())
                {
                    // That doesn't already contain our storage button
                    FieldInfo itemsGet = typeof(GuiMenu).GetField("mItems", BindingFlags.NonPublic | BindingFlags.Instance);
                    var itemsSet = itemsGet.GetValue(itemsGet) as List<GuiMenuItem>;
                    if (itemsSet.Exists(x => x is GuiStorageMenuItem))
                    {
                        // Add our storage button
                        MenuSystem.mMenuAction.mItems.Insert(1, StorageMenuItem);
                    }
                }
            }            
        } 

        public static void OnStorageMenuItemPressed(object parameter)
        {
            //this is mostly to remove the "remove unused parameter if its not part of a shipped public API" message
            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            StorageGuruMod.Game.mActiveModule = (Selection.getSelected() as Planetbase.Module);

            if (StorageGuruMod.GameAccess.mActiveModule is Planetbase.Module module)
            {
                StorageMenu.ActiveStorageModule = module;
                StorageMenu.NeedsRefresh = true;

                StorageGuruMod.GameAccess.mMode = GameStateGame.Mode.CloseCamera;
                Construction selectedConstruction = Selection.getSelectedConstruction();
                CameraManager.getInstance().focusOnPosition(selectedConstruction.getPosition(), selectedConstruction.getRadius() + 10f);
                selectedConstruction.setRenderTop(false);

                MenuSystem.mCurrentMenu = StorageMenu;
            }
        }
    }

    public class GuiStorageMenu : GuiMenu
    {
        public GuiStorageMenuItem StorageMenuItem { get; private set; }
        public bool EnableAll { get; private set; }

        public Planetbase.Module ActiveStorageModule { get; set; }

        public bool NeedsRefresh;

        /// <summary>
        /// Creates a new instance of a generic storage menu without any filters set
        /// </summary>
        public GuiStorageMenu() : base("Storage")
        {

        }

        public void Update()
        {
            if (NeedsRefresh)
            {
                StorageGuruMod.StorageController.ConsolidateManifest();

                if (StorageGuruMod.StorageController.GetManifestEntry(ActiveStorageModule) is ManifestEntry entry)
                {
                    EnableAll = entry.Count != StorageGuruMod.MasterResourceDefinitions.Count && (entry.Count == 0 || EnableAll);

                    clear();

                    // Add toggle for each reasource + style based on whether enabled
                    foreach (var resourceType in StorageGuruMod.MasterResourceDefinitions)
                    {
                        var resourceEnabled = entry.ContainsResource(resourceType);

                        var icon = resourceEnabled ? resourceType.getIcon() : ContentManager.GreyscaleTextures[resourceType.getName()];
                        var tooltip = resourceType.getName() + (resourceEnabled ? " - ON" : " - OFF");

                        addItem(new GuiMenuItem(icon, tooltip, OnResourceToggled, resourceType, GuiMenuItem.FlagMenuSwitch));
                    }

                    if (EnableAll)
                    {
                        addItem(new GuiMenuItem(ContentManager.EnableAllIcon, "Enable All", OnEnableAllToggled));
                    }
                    else
                    {
                        addItem(new GuiMenuItem(ContentManager.DisableAllIcon, "Disable All", OnDisableAllToggled));
                    }

                    addBackItem(new GuiDefinitions.Callback(StorageGuruMod.GameAccess.onButtonCancelEdit));

                    NeedsRefresh = false;
                }
            }
        }

        private void OnResourceToggled(object parameter)
        {
            if(parameter is ResourceType resourceType)
            {
                StorageGuruMod.StorageController.ToggleDefinitions(ActiveStorageModule, resourceType);
                NeedsRefresh = true;
            }
        }

        private void OnEnableAllToggled(object parameter)
        {
            StorageGuruMod.StorageController.AddAllDefinitionsToManifestEntry(ActiveStorageModule);
            NeedsRefresh = true;
        }

        private void OnDisableAllToggled(object parameter)
        {
            StorageGuruMod.StorageController.RemoveAllDefinitionsFromManifestEntry(ActiveStorageModule);
            NeedsRefresh = true;
        }
    }

    /// <summary>
    /// GuiMenuItem subclass so we can easily identify in a menu's items
    /// </summary>
    public class GuiStorageMenuItem : GuiMenuItem
    {
        public GuiStorageMenuItem(GuiStorageMenu menu) : base(
                  TypeList<ModuleType, ModuleTypeList>.find<ModuleTypeStorage>().getIcon(),
                  StringList.get("tooltip_manage_storage"),
                  MenuController.OnStorageMenuItemPressed,
                  menu, FlagMenuSwitch)
        { 

        }
    }
}

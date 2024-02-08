using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StorageGuru
{
    public static class MenuController
    {
        public static GuiStorageMenu StorageMenu;

        public static void Update()
        {
            if (StorageMenu != null)
            {
                StorageMenu.Update();
            }
        }

        internal static void OnBackButtonPressed(object parameter)
        {
            StorageMenu = null;
            ((GameStateGame)GameManager.getInstance().getGameState()).onButtonCancelEdit(parameter);
        }
    }

    public class GuiStorageMenu : GuiMenu
    {
        public GuiStorageMenuItem StorageMenuItem { get; private set; }
        public bool EnableAll { get; private set; }

        public Module ActiveStorageModule { get; set; }

        public bool NeedsRefresh = true;

        /// <summary>
        /// Creates a new instance of a generic storage menu without any filters set
        /// </summary>
        public GuiStorageMenu(Module module) : base("Storage")
        {
            ActiveStorageModule = module;
            MenuController.StorageMenu = this;
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

                    addBackItem(new GuiDefinitions.Callback(MenuController.OnBackButtonPressed));

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
}

using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageGuru
{
    public class GuiStorageMenu : GuiMenu
    {
        public GuiStorageMenuItem StorageMenuItem { get; private set; }

        public GuiStorageMenu(GuiDefinitions.Callback callback) : base("Storage")
        {
            StorageMenuItem = new GuiStorageMenuItem(callback);

            // Add menuItems for each resource
            foreach (var resourceType in StorageGuruMod.ResourceDefinitions)
            {
                var icon = resourceType.getIcon();
                var tooltip = $"{resourceType.getName()} - OFF";

                addItem(new GuiMenuItem(icon, tooltip, OnResourceToggled, resourceType, GuiMenuItem.FlagMenuSwitch));
            }
                        
            addItem(new GuiMenuItem(ContentManager.StorageEnableIcon, "Enable All", OnEnableAllToggled));
        }

        private void OnResourceToggled(object parameter)
        {
            if(parameter is ResourceType resourceType)
            {

            }
        }

        private void OnEnableAllToggled(object parameter)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// GuiMenuItem subclass so we can easily identify in a menu's items
    /// </summary>
    public class GuiStorageMenuItem : GuiMenuItem
    {
        public GuiStorageMenuItem(GuiDefinitions.Callback callback)
            : base(ContentManager.StorageEnableIcon, StringList.get("tooltip_edit"), callback)
        {

        }
    }
}

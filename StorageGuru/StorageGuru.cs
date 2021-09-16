using Planetbase;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace StorageGuru
{
    public class StorageGuru : IMod
    { 
        private static StorageGuru _instance;
        public static StorageGuru GetInstance() { return _instance; }
        public static string STORAGE_MANIFEST_PATH = @"Mods\Settings\storage_manifest.txt";

        public static StorageController Controller { get; private set; }

        private GameStateGame Game;
        private Module ActiveModule;

        private GuiMenuSystem MenuSystem;
        private GuiStorageMenuItem StorageMenuItem;

        public static List<ResourceType> ResourceDefinitions { get; private set; }
         
        bool initialising = true;
        private bool enableAll = true;

        public StorageGuru()
        {
        }

        public void Init()
        {
            _instance = this;

            STORAGE_MANIFEST_PATH = Path.Combine(Util.getFilesFolder(), STORAGE_MANIFEST_PATH);
            ContentManager.LoadContent();

            ResourceDefinitions = TypeList<ResourceType, ResourceTypeList>.getInstance().mTypeList
                .Where(x => !(x is Coins)).ToList();  // Clear out non-storeable resources (coins)

            StorageMenuItem = new GuiStorageMenuItem((object o) => OnButtonStorage());

            Debug.Log("[MOD] Storage Guru activated");
        }

        public void Update()
        {
            Game = GameManager.getInstance().getGameState() as GameStateGame;

            if (Game != null)
            {
                if (initialising)
                {
                    MenuSystem = Game.mMenuSystem;

                    FirstUpdate();
                    initialising = false;
                }

                GameUpdate();

                Controller.ConsolidateDefinitions();
            }
        }

        private void GameUpdate()
        {
            // If we're viewing an action menu
            if (MenuSystem != null && MenuSystem.mMenuAction is GuiMenu actionMenu)
            {
                // That is for a storage module
                if (Selection.getSelected() is Module module && module.getModuleType() is ModuleTypeStorage)
                {
                    // That doesn't already contain our storage button
                    if (!actionMenu.mItems.Exists(x => x is GuiStorageMenuItem))
                    { 
                        // Add our storage button
                        MenuSystem.mMenuAction.mItems.Insert(1, StorageMenuItem);
                    }
                }
            }

            RedirectCharacters();
        }

        private void OnButtonStorage()
        {
            Game.mActiveModule = Selection.getSelected() as Module;
            ActiveModule = Game.mActiveModule;

            if (Game.mActiveModule != null)
            {
                SetupEditMenu();
                Game.mMode = GameStateGame.Mode.CloseCamera;
                Construction selectedConstruction = Selection.getSelectedConstruction();
                CameraManager.getInstance().focusOnPosition(selectedConstruction.getPosition(), selectedConstruction.getRadius() + 10f);
                selectedConstruction.setRenderTop(false);
            }
        }

        public void FirstUpdate()
        {
            Controller = new StorageController();
            Controller.LoadManifest(STORAGE_MANIFEST_PATH);
            initialising = false;
        }

        private void SetupEditMenu()
        {
            var menu = new GuiMenu("Storage");
            var definitions = Controller.GetDefinitions(ActiveModule);

            foreach (var resource in ResourceDefinitions)
            {
                var icon = resource.getIcon();
                var tooltip = resource.getName();

                if (definitions.Count == 0 || !definitions.Contains(resource.GetType()))
                { 
                    tooltip += " - OFF";
                }
                else
                { 
                    tooltip += " - ON";
                }

                if (icon != null)
                {
                    menu.addItem(new GuiMenuItem(icon, tooltip, (object o) =>
                    {
                        if (o is ResourceType resourceType)
                        {
                            throw new NotImplementedException();
                        }
                        Controller.ToggleDefinitions(ActiveModule, resource.GetType());s
                        SetupEditMenu();
                    }, resource, GuiMenuItem.FlagMenuSwitch));
                }
            }

            enableAll = definitions.Count != ResourceDefinitions.Count && (definitions.Count == 0 || enableAll);

            if(enableAll)
            {
                //menu.addItem(new GuiMenuItem(ContentManager.StorageEnableIcon, "Enable All", EnableAllCallback));
            }
            else
            {
                //menu.addItem(new GuiMenuItem(ContentManager.StorageDisableIcon, "Disable All", DisableAllCallback));
            }

            menu.addBackItem(new GuiDefinitions.Callback(Game.onButtonCancelEdit));
            MenuSystem.mCurrentMenu = menu;
        }

        public void StorageCallback<T>(object parameter) where T : ResourceType
        {
            Controller.ToggleDefinitions(ActiveModule, typeof(T));
            SetupEditMenu();
        }

        private Dictionary<Character, Resource> carriedResources;
        private Dictionary<Character, Resource> newCarriedResources;
        private List<Type> uniqueCarriedResources;
        private Dictionary<Type, List<Module>> resourceTargets;
        private Dictionary<Character, Module> newCharacterTargets;

        private Dictionary<Character, Module> characterTargets = new Dictionary<Character, Module>();

        private void RedirectCharacters()
        {
            carriedResources = Character.mCharacters.Where(x => x.getLoadedResource() != null).ToDictionary(y => y, x => x.getLoadedResource()); // Get all carried resources
            newCarriedResources = carriedResources.Where(x => !characterTargets.ContainsKey(x.Key)).ToDictionary(y => y.Key, x => x.Value); // That haven't been processed

            if(newCarriedResources.Count == 0) { return; }

            newCarriedResources = newCarriedResources.Where(x => 
            x.Key.getTarget() is Target target // That are valid storage
            && target.getSelectable() is Module module
            && module.isBuilt() 
            && module.getCategory() == Module.Category.Storage)
                .ToDictionary(x => x.Key, y => y.Value);

            uniqueCarriedResources = newCarriedResources.Select(x => x.Value.getResourceType().GetType()).Distinct().ToList(); // Get unique carried resource types

            resourceTargets = uniqueCarriedResources.ToDictionary(x => x, y => Controller.GetValidModules(y)); // Get possible targets for resource types

            newCharacterTargets = newCarriedResources.ToDictionary(x => x.Key, y => Controller.FindNearestModule(y.Key.getPosition(), // Find nearest
                resourceTargets[y.Value.getResourceType().GetType()]));

            characterTargets = characterTargets.Where(x => newCarriedResources.ContainsKey(x.Key)).ToDictionary(x => x.Key, y => y.Value); // Removed completed charcaters from list

            foreach(var kvp in newCharacterTargets)
            {
                if(kvp.Value != null)
                {
                    kvp.Key.setTarget(new Target(kvp.Value, kvp.Value.getRadius() / 1.8f));
                }

                characterTargets.Add(kvp.Key, kvp.Value);
            }
        }

        public static void RefreshStorage(Module module, HashSet<Type> allowedResources)
        {
            if (module == null) { return; }

            if (module.mResourceStorage != null && module.mResourceStorage.mSlots != null)
            {
                foreach (var slot in module.mResourceStorage.mSlots)
                {
                    if (slot != null && slot.mResources != null)
                    {
                        foreach (var res in slot.mResources)
                        {
                            if (allowedResources.Contains(res.getResourceType().GetType()))
                            {
                                res.setState(Resource.State.Stored);
                            }
                            else if (Controller.IsStorageAvailable(res.getResourceType().GetType()))
                            {
                                res.setState(Resource.State.Idle);
                            }
                        }
                    }
                }
            }
        }
    }
}

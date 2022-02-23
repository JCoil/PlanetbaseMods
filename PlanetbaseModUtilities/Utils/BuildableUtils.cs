using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanetbaseModUtilities
{
    /// <summary>
    /// Helper methods and members for Modules, Constructions, ConstructionComponents
    /// </summary>
    public static class BuildableUtils
    {
        public static List<Construction> GetAllConstructions()
        {
            return CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");
        }

        public static List<Module> GetAllModules()
        {
            return CoreUtils.GetMember<Module, List<Module>>("mModules");
        }

        #region Modules

        public static T FindModuleType<T>() where T : ModuleType
        {
            return (T)TypeList<ModuleType, ModuleTypeList>.find<T>();
        }

        public static Module.Category GetCategory(this Module module) 
        {
            return CoreUtils.InvokeMethod<Module, Module.Category>("getCategory", module);
        }

        /// <summary>
        /// Category is infered from other fields on module, but only determined at init time.
        /// After that, Category is recovered by querying static mModuleCategories, so to change an existing module's category, we must update mModuleCategories
        /// </summary>
        public static void SetCategory(this Module module, Module.Category category)
        {
            var mModuleCategories = CoreUtils.GetMember<Module, List<Module>[]>("mModuleCategories");

            // Find and remove old category entry
            foreach(var categoryList in mModuleCategories)
            {
                if(categoryList.Contains(module))
                {
                    categoryList.Remove(module);
                }
            }

            // Make sure the category list exists (This shouldn't ever come up - should be set early in the game lifecycle)
            if (mModuleCategories[(int)category] == null)
            {
                mModuleCategories[(int)category] = new List<Module>();
            }

            mModuleCategories[(int)category].Add(module);
        }

        #endregion

        #region Components

        public static List<ConstructionComponent> GetAllComponents()
        {
            return CoreUtils.GetMember<ConstructionComponent, List<ConstructionComponent>>("mComponents");
        }

        public static List<ComponentType> GetComponentTypes(this ModuleType moduleType)
        {
            return CoreUtils.GetMember<ModuleType, ComponentType[]>("mComponentTypes", moduleType).ToList();
        }

        public static void SetComponentTypes(this ModuleType moduleType, List<ComponentType> componentTypes)
        {
            CoreUtils.SetMember("mComponentTypes", moduleType, componentTypes.ToArray());
        }

        #endregion

        #region Storage

        public static ResourceStorage GetResourceStorageObject(this Module module)
        {
            return CoreUtils.GetMember<Module, ResourceStorage>("mResourceStorage", module);
        }

        public static List<StorageSlot> GetSlots(this ResourceStorage storage)
        {
            return CoreUtils.GetMember<ResourceStorage, List<StorageSlot>>("mSlots", storage);
        }

        public static List<Resource> GetResources(this StorageSlot slot)
        {
            return CoreUtils.GetMember<StorageSlot, List<Resource>>("mResources", slot);
        }

        #endregion
    }
}

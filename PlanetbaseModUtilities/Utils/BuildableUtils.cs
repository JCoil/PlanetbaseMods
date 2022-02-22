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

        #region Modules

        public static T FindModuleType<T>() where T : ModuleType
        {
            return (T)TypeList<ModuleType, ModuleTypeList>.find<T>();
        }

        public static Module.Category GetCategory(this Module module) 
        {
            return CoreUtils.InvokeMethod<Module, Module.Category>("getCategory", module);
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

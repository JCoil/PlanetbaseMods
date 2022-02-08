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

        public static List<ConstructionComponent> GetAllComponents()
        {
            return CoreUtils.GetMember<ConstructionComponent, List<ConstructionComponent>>("mComponents");
        }

        public static T FindModuleType<T>() where T:ModuleType
        {
            return (T)TypeList<ModuleType, ModuleTypeList>.find<T>();
        }

        public static List<ComponentType> GetComponentTypes<T>(this T moduleType) where T : ModuleType
        {
            return CoreUtils.GetMember<T, ComponentType[]>("mComponentTypes", moduleType).ToList();
        }

        public static void SetComponentTypes<T>(this T moduleType, List<ComponentType> componentTypes) where T : ModuleType
        {
            CoreUtils.SetMember("mComponentTypes", moduleType, componentTypes.ToArray());
        }
    }
}

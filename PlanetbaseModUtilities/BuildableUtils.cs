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
        public static List<Construction> GetConstructions() => 
            CoreUtils.GetMember<Construction, List<Construction>>("mConstructions");

        public static List<ConstructionComponent> GetComponents() => 
            CoreUtils.GetMember<ConstructionComponent, List<ConstructionComponent>>("mComponents");

        public static List<ComponentType> GetComponentTypes(ModuleType moduleType)
        {
            return CoreUtils.GetMember<ModuleType, ComponentType[]>("mComponentTypes", moduleType).ToList();
        }

        public static void SetComponentTypes(ModuleType moduleType, List<ComponentType> componentTypes)
        {
            CoreUtils.SetMember<ModuleType, ComponentType[]>("mComponentTypes", moduleType, componentTypes.ToArray());
        }
    }
}

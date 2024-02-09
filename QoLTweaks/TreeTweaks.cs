using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlanetbaseModUtilities;
using Planetbase;


namespace QoLTweaks
{
    public static class TreeTweaks
    {
        public static void AllowTreesInMultiDomes()
        {
            // Add tree components to ModuleTypeMultiDome 
            var multiDomeComponents = ModuleTypeList.find<ModuleTypeMultiDome>().GetComponentTypesSizes();

            var size1Components = multiDomeComponents[1].ToList();
            size1Components.Add(TypeList<ComponentType, ComponentTypeList>.find<OakTree>());
            size1Components.Add(TypeList<ComponentType, ComponentTypeList>.find<PineTree>());
            multiDomeComponents[1] = size1Components.ToArray();

            var size2Components = multiDomeComponents[2].ToList();
            size2Components.Add(TypeList<ComponentType, ComponentTypeList>.find<OakTree>());
            size2Components.Add(TypeList<ComponentType, ComponentTypeList>.find<PineTree>());
            multiDomeComponents[2] = size2Components.ToArray();


            ModuleTypeList.find<ModuleTypeMultiDome>().SetComponentTypesSizes(multiDomeComponents);
        }
    }
}

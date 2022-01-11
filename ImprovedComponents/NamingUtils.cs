using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImprovedComponents
{
    public static class NamingUtils
    {
        // We could hardcode these as they're straightforward, but for thoroughness, we'll extract them at runtime from the base types

        public static string MetalProcessorTypeName => 
            Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<MetalProcessor>().GetType().Name);

        public static string BioplasticProcessorTypeName =>
            Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<BioplasticProcessor>().GetType().Name);

        public static string MedicinalPadTypeName =>
            Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<MedicinalPad>().GetType().Name);

        public static string WheatPadTypeName =>
            Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<WheatPad>().GetType().Name);

        public static string RicePadTypeName =>
            Util.camelCaseToLowercase(TypeList<ComponentType, ComponentTypeList>.find<RicePad>().GetType().Name);
    }
}

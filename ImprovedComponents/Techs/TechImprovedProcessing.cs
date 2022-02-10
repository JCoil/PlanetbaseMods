using Planetbase;
using PlanetbaseModUtilities;
using UnityEngine;

namespace ImprovedComponents
{
    public class TechImprovedProcessing : Tech
	{
        // Token: 0x06000BA2 RID: 2978 RVA: 0x000412E6 File Offset: 0x0003F4E6
        public TechImprovedProcessing()
        {
            load();
            this.mMerchantCategory = MerchantCategory.Industrial;
            this.mValue = 400;
        }

        public new void load()
        {
            this.initStrings();
            this.mIcon = ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MetalProcessorTypeName, Color.cyan); // Borrow icon from Metal Processor
        }

        public static void RegisterStrings()
        {
            StringUtils.RegisterString("tech_improved_processing", "Improved Processing");
            StringUtils.RegisterString("tech_improved_processing_description", 
                "Allows you to construct enhanced processors, which refine raw materials more efficiently at the cost of much higher power usage");
        }
    }
}

﻿using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ImprovedComponents
{
    public class TechGmMedicinalPlants : Tech
    {
        // Token: 0x06000BA2 RID: 2978 RVA: 0x000412E6 File Offset: 0x0003F4E6
        public TechGmMedicinalPlants()
        {
            load();
            this.mMerchantCategory = MerchantCategory.Medical;
            this.mValue = 600;
        }

        public new void load()
        {
            this.initStrings();
            this.mIcon = ResourceUtil.loadIconColor("Components/icon_" + NamingUtils.MedicinalPadTypeName, Color.cyan); 
        }

        public static void RegisterStrings()
        {
            StringList.mStrings.Add("tech_gm_medicinal_plants", "GM Medicinal Plants DNA");
            StringList.mStrings.Add("tech_gm_medicinal_plants_description", "Allows you to plant fast-growing GM Medicinal Plants");
        }
    }
}
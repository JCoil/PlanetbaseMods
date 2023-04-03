using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace QoLTweaks
{
    [HarmonyPatch(typeof(ResourceInfo), nameof(ResourceInfo.setAmount))]
    public class LargeNumbersDisplayPatch
    {
        public static void Postfix(ref GUIContent ___mContent, ResourceType resourceType, int amount, int freeAmount)
        {
            // End of method that we're fixing
            //if (amount < 10000)
            //{
            //    this.mContent.text = this.mFreeAmount.ToString();
            //}
            //else
            //{
            //    this.mContent.text = StringList.get("infinity");
            //}
            //this.mContent.image = resourceType.getIcon();

            ___mContent.text = KiloFormat(amount);
        }

        public static string KiloFormat(int num)
        {
            if (num + 500000 >= 100000000) // 100 million
                return StringList.get("infinity");

            if (num + 500000 >= 10000000) // 10 million
                return ((num + 500000) / 1000000).ToString("#0M");

            if (num + 500 >= 1000000) // 1 million
                return ((num + 500) / 1000000).ToString("0.#") + "M";

            if (num + 500 >= 100000) // 100 thousand
                return ((num + 500) / 1000).ToString("#0k");

            if (num >= 10000) // 10 thousand
                return ((num + 500) / 1000).ToString("0.#") + "k";

            return num.ToString("#0");
        }
    }
}

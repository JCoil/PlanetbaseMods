using HarmonyLib;
using Planetbase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace CheatTools.Patches
{
    [HarmonyPatch(typeof(Module))]
    public class ModulePatches
    {
        [HarmonyPostfix]
        [HarmonyPatch("canPlaceModule")]
        public static void Postfix1(ref bool __result)
        {
            if (CheatTools.ModuleAnarchyOn)
            {
                __result = true;
            }
        }
    }

    //[HarmonyPatch(typeof(Connection))]
    //public class ConnectionPatches
    //{
    //    [HarmonyTranspiler]
    //    [HarmonyPatch(argumentTypes: new Type[] { typeof(Module), typeof(Module), typeof(Vector3), typeof(Vector3) }, methodName: nameof(Connection.canLink))]
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var code = new List<CodeInstruction>(instructions);

    //        // We'll need to modify code here.
    //        if (code.FirstOrDefault(x => x.opcode == OpCodes.Ldc_R4 && (float)x.operand == 20f) is CodeInstruction instruction)
    //        {
    //            instruction.operand = 100f;
    //        }

    //        return code;
    //    }
    //}
}

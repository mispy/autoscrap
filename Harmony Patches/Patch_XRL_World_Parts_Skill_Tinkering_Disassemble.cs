using HarmonyLib;
using QudUX.Concepts;
using XRL.World;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.Parts.Skill.Tinkering_Disassemble))]
    class Patch_XRL_World_Parts_Skill_Tinkering_Disassemble
    {
        [HarmonyPostfix]
        [HarmonyPatch("WantToDisassemble")]
        static void Postfix(GameObject obj,  XRL.World.Parts.Skill.Tinkering_Disassemble __instance, ref bool __result)
        {
            if (__result) {
                // Vanilla scrap disassembly
                __result = true;
            } else {
                __result = __instance.ParentObject.IsPlayer() && XRL.World.Parts.QudUX_AutogetHelper.WantToDisassemble(obj); 
            }
        }
    }
}

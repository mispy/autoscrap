using HarmonyLib;
using QudUX.Concepts;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.Parts.Skill.Tinkering_Disassemble))]
    class Patch_XRL_World_Parts_Skill_Tinkering_Disassemble
    {
        [HarmonyPostfix]
        [HarmonyPatch("WantToDisassemble")]
        static void Postfix(XRL.World.GameObject __instance, ref bool __result)
        {
            __result = true;
            // if (__result == true && Options.UI.EnableAutogetExclusions)
            // {
            //     __result = !XRL.World.Parts.QudUX_AutogetHelper.IsAutogetDisabledByQudUX(__instance);
            // }
        }
    }
}

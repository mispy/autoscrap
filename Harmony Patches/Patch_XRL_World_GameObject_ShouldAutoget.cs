  
using HarmonyLib;
using XRL.World.Parts;
using XRL.UI;

namespace Autoscrap.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.GameObject))]
    class Patch_XRL_World_GameObject_ShouldAutoget
    {
        [HarmonyPostfix]
        [HarmonyPatch("ShouldAutoget")]
        static void Postfix(XRL.World.GameObject __instance, ref bool __result)
        {
            // Make autodisassembly-tagged items also autoget
            if (!__result) {
                __result = Options.AutogetScrap && Autoscrap_AutodisassemblyHelper.WantToDisassemble(__instance); 
            }
        }
    }
}
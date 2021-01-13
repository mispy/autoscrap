using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using XRL.Core;
using XRL.World.Parts;

namespace QudUX.HarmonyPatches
{   
    [HarmonyPatch(typeof(XRL.World.ZoneManager))]
    class Patch_XRL_World_ZoneManager
    {
        [HarmonyPostfix]
        [HarmonyPatch("Tick")]
        static void Postfix()
        {
            var player = XRLCore.Core.Game.Player.Body;
            player.RequirePart<QudUX_AutogetHelper>();
        }
    }
}

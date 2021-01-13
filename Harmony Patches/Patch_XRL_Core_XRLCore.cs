using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static QudUX.HarmonyPatches.PatchHelpers;
using static QudUX.Concepts.Constants.MethodsAndFields;

namespace QudUX.HarmonyPatches
{   
    [HarmonyPatch(typeof(XRL.Core.XRLCore))]
    class Patch_XRL_Core_XRLCore
    {
        [HarmonyTranspiler]
        [HarmonyPatch("NewGame")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var Sequence = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldstr, "Starting game!")
            });

            bool patched = false;
            foreach (var instruction in instructions)
            {
                if (!patched && Sequence.IsMatchComplete(instruction))
                {
                    yield return new CodeInstruction(OpCodes.Call, Events_OnLoadAlwaysEvent);
                    patched = true;
                }
                yield return instruction;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("LoadGame")]
        static void Postfix()
        {
            try
            {
                QudUX.Concepts.Events.OnLoadAlwaysEvent();
            }
            catch { }
        }
    }
}

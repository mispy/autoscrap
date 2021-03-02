using System;
using XRL;
using XRL.Language;
using XRL.UI;
using XRL.Core;
using System.IO;

namespace XRL.World.Parts
{
    [Serializable]
    public class Autoscrap_AutodisassemblyHelper : IPart
    {
        public static readonly string CmdDisableAutodisassemble = "Autoscrap_DisableItemAutodisassemble";
        public static readonly string CmdEnableAutodisassemble = "Autoscrap_EnableItemAutodisassemble";

        public static bool WantToDisassemble(GameObject obj)
        {
            bool enabled = XRLCore.Core.Game.GetStringGameState($"Autoscrap_ShouldAutodisassemble:{obj.Blueprint}").EqualsNoCase("Yes");
            return enabled && obj.IsValid() && CanToggleAutoDisassemble(obj);
        }

        public static bool CanToggleAutoDisassemble(GameObject obj)
        {
            var tinkerable = obj.GetPart<TinkerItem>();

            if (tinkerable == null || !tinkerable.CanBeDisassembled())
                return false;

            if (obj.GetIntProperty("Scrap") > 0)
                return false; // Don't interfere with vanilla scrap disassembly

            if (obj.HasTagOrProperty("QuestItem") || obj.HasProperName)
                return false; // Don't disassemble quest items or artifacts

            if (!obj.IsTakeable())
                return false; // Can't autodisassemble hyperbiotic chairs for now

            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == OwnerGetInventoryActionsEvent.ID || ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(OwnerGetInventoryActionsEvent E)
        {
            if (CanToggleAutoDisassemble(E.Object))
            {
                if (WantToDisassemble(E.Object))
                {
                    E.AddAction("Enable autodisassembly on pickup for this item", "disable autoscrap", CmdDisableAutodisassemble, FireOnActor: true);
                }
                else
                {
                    E.AddAction("Disable autodisassembly on pickup for this item", "enable autoscrap", CmdEnableAutodisassemble, FireOnActor: true);
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == CmdEnableAutodisassemble)
            {
                XRLCore.Core.Game.SetStringGameState($"Autoscrap_ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
            } 
            else if (E.Command == CmdDisableAutodisassemble)
            {
                XRLCore.Core.Game.RemoveStringGameState($"Autoscrap_ShouldAutodisassemble:{E.Item.Blueprint}");
            }
            return base.HandleEvent(E);
        }
    }
}
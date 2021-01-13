using System;
using XRL.Language;
using XRL.UI;
using XRL.Core;
using System.IO;
using Constants = Autoscrap.Concepts.Constants;

namespace XRL.World.Parts
{
    [Serializable]
    public class Autoscrap_AutodisassemblyHelper : IPart
    {
        private static NameValueBag _AutoscrapSettings;
        private static string LoadedSettingsGameID;
        public static NameValueBag AutoscrapSettings
        {
            get
            {
                var currentGameID = XRLCore.Core.Game.GameID;
                if (LoadedSettingsGameID != currentGameID)
                {
                    // Settings are per-save
                    var path = Path.Combine(Constants.ModDirectory, $"Autoscrap_{currentGameID}.json");
                    _AutoscrapSettings = new NameValueBag(path);
                    _AutoscrapSettings.Load();
                    LoadedSettingsGameID = currentGameID;
                }
                return _AutoscrapSettings;
            }
        }
        public static readonly string CmdDisableAutodisassemble = "Autoscrap_DisableItemAutodisassemble";
        public static readonly string CmdEnableAutodisassemble = "Autoscrap_EnableItemAutodisassemble";

        public static bool WantToDisassemble(GameObject obj)
        {
            bool enabled = AutoscrapSettings.GetValue($"ShouldAutodisassemble:{obj.Blueprint}", "").EqualsNoCase("Yes");
            return enabled && obj.IsValid() && CanToggleAutoDisassemble(obj);
        }

        public static bool CanToggleAutoDisassemble(GameObject obj)
        {
            var tinkerable = obj.GetPart<TinkerItem>();

            if (tinkerable == null || !tinkerable.CanBeDisassembled())
                return false;

            if (obj.GetIntProperty("Scrap") > 0)
                return false; // Don't interfere with vanilla scrap disassembly

            if (obj.HasTagOrProperty("QuestItem"))
                return false;

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
                    E.AddAction("Enable autodisassembly on pickup for this item", "disable autodisassembly", CmdDisableAutodisassemble, FireOnActor: true);
                }
                else
                {
                    E.AddAction("Disable autodisassembly on pickup for this item", "enable autodisassembly", CmdEnableAutodisassemble, FireOnActor: true);
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == CmdEnableAutodisassemble)
            {
                AutoscrapSettings.SetValue($"ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
            }
            if (E.Command == CmdDisableAutodisassemble)
            {
                AutoscrapSettings.Bag.Remove($"ShouldAutodisassemble:{E.Item.Blueprint}");
                AutoscrapSettings.Flush();
            }
            return base.HandleEvent(E);
        }
    }
}
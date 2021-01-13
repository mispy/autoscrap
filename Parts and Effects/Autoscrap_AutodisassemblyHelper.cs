using System;
using XRL.Language;
using XRL.UI;
using Constants = Autoscrap.Concepts.Constants;

namespace XRL.World.Parts
{
    [Serializable]
    public class Autoscrap_AutodisassemblyHelper : IPart
    {
        private static NameValueBag _AutoscrapSettings;
        public static NameValueBag AutoscrapSettings
        {
            get
            {
                if (_AutoscrapSettings == null)
                {
                    _AutoscrapSettings = new NameValueBag(Constants.AutoscrapDataFilePath);
                    _AutoscrapSettings.Load();
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
                bool bInfoboxShown = AutoscrapSettings.GetValue("Metadata:InfoboxWasShown", "").EqualsNoCase("Yes");
                if (!bInfoboxShown)
                {
                    DialogResult choice = DialogResult.Cancel;
                    while (choice != DialogResult.Yes && choice != DialogResult.No)
                    {
                        choice = Popup.ShowYesNo("Enabling auto-disassembly for " + Grammar.Pluralize(E.Item.DisplayNameOnly) + ".\n\n"
                            + "Changes to auto-pickup preferences will apply to ALL of your characters. "
                            + "If you proceed, this message will not be shown again.\n\nProceed?", false, DialogResult.Cancel);
                    }
                    if (choice == DialogResult.Yes)
                    {
                        AutoscrapSettings.SetValue("Metadata:InfoboxWasShown", "Yes", FlushToFile: false);
                        AutoscrapSettings.SetValue($"ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
                    }
                }
                else
                {
                    AutoscrapSettings.SetValue($"ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
                }
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
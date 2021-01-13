using System;
using XRL.Language;
using XRL.UI;
using QudUX_Constants = QudUX.Concepts.Constants;

namespace XRL.World.Parts
{
    [Serializable]
    public class QudUX_AutogetHelper : IPart
    {
        private static bool TemporarilyIgnoreQudUXSettings;
        private static NameValueBag _AutogetSettings;
        public static NameValueBag AutogetSettings
        {
            get
            {
                if (_AutogetSettings == null)
                {
                    _AutogetSettings = new NameValueBag(QudUX_Constants.AutogetDataFilePath);
                    _AutogetSettings.Load();
                }
                return _AutogetSettings;
            }
        }
        public static readonly string CmdDisableAutodisassemble = "QudUX_DisableItemAutodisassemble";
        public static readonly string CmdEnableAutodisassemble = "QudUX_EnableItemAutodisassemble";

        public static bool WantToDisassemble(GameObject obj)
        {
            if (TemporarilyIgnoreQudUXSettings)
            {
                return false;
            }

            bool enabled = AutogetSettings.GetValue($"ShouldAutodisassemble:{obj.Blueprint}", "").EqualsNoCase("Yes");
            return enabled && obj.IsValid() && obj.HasPart("TinkerItem") && obj.Understood() && 
                   !obj.HasTagOrProperty("QuestItem");
        }

        public static bool CanToggleAutoDisassemble(GameObject obj)
        {
            if (obj.HasPart("TinkerItem")) {
                return obj.Understood() && obj.GetIntProperty("Scrap") != 1;
            } else {
                return false;
            }
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
                    E.AddAction("Enable autodisassembly on pickup for this item", "disable autodisassemble", CmdDisableAutodisassemble, FireOnActor: true);
                }
                else
                {
                    E.AddAction("Disable autodisassembly on pickup for this item", "enable autodisassemble", CmdEnableAutodisassemble, FireOnActor: true);
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            if (E.Command == CmdEnableAutodisassemble)
            {
                bool bInfoboxShown = AutogetSettings.GetValue("Metadata:InfoboxWasShown", "").EqualsNoCase("Yes");
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
                        AutogetSettings.SetValue("Metadata:InfoboxWasShown", "Yes", FlushToFile: false);
                        AutogetSettings.SetValue($"ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
                    }
                }
                else
                {
                    AutogetSettings.SetValue($"ShouldAutodisassemble:{E.Item.Blueprint}", "Yes");
                }
            }
            if (E.Command == CmdDisableAutodisassemble)
            {
                AutogetSettings.Bag.Remove($"ShouldAutodisassemble:{E.Item.Blueprint}");
                AutogetSettings.Flush();
            }
            return base.HandleEvent(E);
        }
    }
}
using System;
using System.Reflection;
using System.Collections.Generic;
using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Messages;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Skills.Cooking;
using static HarmonyLib.SymbolExtensions;
using static HarmonyLib.AccessTools;
using System.IO;

namespace Autoscrap.Concepts
{
    [HasModSensitiveStaticCache]
    public static class Constants
    {
        public static string AutoscrapDataFileName => "Autoscrap_AutodisassemblySettings.json";

        public static string AutoscrapDataFilePath => Path.Combine(ModDirectory, AutoscrapDataFileName);

        private static string _modDirectory = null;
        public static string ModDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_modDirectory))
                {
                    ModManager.ForEachMod(delegate (ModInfo mod)
                    {
                        if (mod?.manifest?.id == "AutoscrapAnything" || mod?.workshopInfo?.Title == "AutoscrapAnything")
                        {
                            _modDirectory = mod.Path;
                            return;
                        }
                    });
                }
                return _modDirectory;
            }
        }
    }
}

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

namespace QudUX.Concepts
{
    [HasModSensitiveStaticCache]
    public static class Constants
    {
        public static string AutogetDataFileName => "QudUX_AutogetSettings.json";

        public static string AutogetDataFilePath => Path.Combine(ModDirectory, AutogetDataFileName);

        private static string _modDirectory = null;
        public static string ModDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(_modDirectory))
                {
                    ModManager.ForEachMod(delegate (ModInfo mod)
                    {
                        if (mod?.manifest?.id == "QudUX" || mod?.workshopInfo?.Title == "QudUX")
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

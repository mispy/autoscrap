using XRL.Core;
using XRL.World;
using XRL.World.Parts;

namespace QudUX.Concepts
{
    //Custom events that are called from Patch_XRL_Core_XRLCore.
    //The player object (XRLCore.Core.Game.Player.Body) is available for use in all of these events.
    public static class Events
    {
        private static GameObject Player => XRLCore.Core?.Game?.Player?.Body;

        //Runs in all load scenarios - called immediately after each of the events above.
        public static void OnLoadAlwaysEvent()
        {
            if (Player != null)
            {
                Player.RequirePart<QudUX_AutogetHelper>();
            }
        }
    }
}

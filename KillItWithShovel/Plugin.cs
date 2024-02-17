using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace KillItWithShovel
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {

        
       
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
                // Plugin startup logic
                Logger.LogInfo($"Plugin {modGUID} is loaded!");

            

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            //mls.LogInfo("TestMod has awaken");

            this.harmony.PatchAll(typeof(Plugin));
            this.harmony.PatchAll(typeof(Patches.TurretPatch));
            this.harmony.PatchAll(typeof(Config));

            MyConfig = new(base.Config);
        }

        // Token: 0x04000001 RID: 1
        private const string modGUID = "MiffyL.KillItWithShovel";

        // Token: 0x04000002 RID: 2
        private const string modName = "KillItWithShovel";

        // Token: 0x04000003 RID: 3
        private const string modVersion = "1.0.0";

        // Token: 0x04000004 RID: 4
        private readonly Harmony harmony = new Harmony("MiffyL.KillItWithShovel");

        // Token: 0x04000005 RID: 5
        public static Plugin Instance;

        // Token: 0x04000006 RID: 6
        internal ManualLogSource mls;

        // Token: 0x04000007 RID: 7
        public static Config MyConfig { get; internal set; }

        // Token: 0x04000008 RID: 8
        //public ConfigEntry<float> configDisableDuration;

        // Token: 0x04000009 RID: 9
        //private readonly Harmony harmony = new Harmony("MiffyL.KillItWithShovel");

        // Token: 0x0400000A RID: 10
        //internal static Plugin Instance;

    }
}
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Netcode;

namespace KillItWithShovel
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {

        public static ConfigEntry<bool> configEnterBerserkMode;

        public bool bMode;

        public static ConfigEntry<float> configDisableDuration;

        public float dDuration;


        public Config(ConfigFile cfg)
        {
            InitInstance(this); // Add this line

            
            configEnterBerserkMode = cfg.Bind<bool>(
                "General",
                "Berseker Mode when being waked",
                true,
                "Turret will enter berseker mode if being rudely waken by weapons. (Dafault: true)");

            configDisableDuration = cfg.Bind<float>(
                "General",
                "Disable Duration",
                5f,
                "How many seconds the turret will be disabled. (Dafault: 5)");

        }

        public static void BuildServerConfigSync()
        {
            Default.bMode = configEnterBerserkMode.Value;
            Default.dDuration = configDisableDuration.Value;

            Instance.dDuration = Default.dDuration;
            Instance.bMode = Default.bMode;
        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("KillItWithShovel_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            mls2.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("KillItWithShovel_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                mls2.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }


        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                mls2.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                mls2.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            mls2.LogInfo("Successfully synced config with host.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            //sync to others
            if (IsHost)
            {
                BuildServerConfigSync();
                MessageManager.RegisterNamedMessageHandler("KillItWithShovel_OnRequestConfigSync", OnRequestSync);
                Synced = true;

                return;
            }

            //get sync from host
            Synced = false;
            MessageManager.RegisterNamedMessageHandler("KillItWithShovel_OnReceiveConfigSync", OnReceiveSync);
            RequestSync();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            Config.RevertSync();
        }
    }
}

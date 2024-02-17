using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static KillItWithShovel.Config;

namespace KillItWithShovel.Patches
{

    [HarmonyPatch(typeof(Turret))]
    internal class TurretPatch
    {
        [HarmonyPatch(typeof(Turret),"IHittable.Hit")]

        //prefix + return false = skip running of original code
        //post fix = original code + modified code
        [HarmonyPrefix]

        

        public static bool HitDisablePatch(ref TurretMode ___turretMode, ref Turret __instance)
        {


            bool flag = TurretPatch.turretHitPatchMB == null;
            if (flag)
            {

                TurretPatch.turretHitPatchMB = __instance.gameObject.AddComponent<TurretPatch.TurretHitPatchMB>();
            }


            if (!__instance.turretActive || ___turretMode == TurretMode.Berserk)
            {
                if (Config.Instance.bMode)
                {
                    __instance.ToggleTurretEnabled(true);
                    return true;
                }
                return false;
            }
            else
            {
                //if player hit it while it is in beserker mode
                //if(___turretMode == TurretMode.Berserk) return false;
                turretHitPatchMB.StartCoroutine(TurretPatch.TurnOffAndOnTurret(__instance, Config.Instance.dDuration));
            }




            return false;

        }

        public static IEnumerator TurnOffAndOnTurret(Turret _turret, float duration)
        {
            
            _turret.ToggleTurretEnabled(false);

            yield return new WaitForSeconds(duration);

            _turret.ToggleTurretEnabled(true);

            yield break;
        }

        private static TurretPatch.TurretHitPatchMB turretHitPatchMB;

        public class TurretHitPatchMB: MonoBehaviour { }
       
    }
}

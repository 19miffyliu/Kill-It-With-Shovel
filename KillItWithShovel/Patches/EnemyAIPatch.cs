using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace KillItWithShovel.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]


    internal class EnemyAIPatch
    {
        [HarmonyPatch(typeof(EnemyAI), "HitEnemy")]
        [HarmonyPrefix]
        public static bool HitPushBackPatch(ref int force, ref PlayerControllerB playerWhoHit, ref EnemyAI __instance)
        {
            

            switch (__instance.enemyType.enemyName)
            {
                case "SpringManAI":
                case "JesterAI":
                    PushBackHit(ref force, ref playerWhoHit, ref __instance);
                    break;
                    

                default:
                    break;
            }


            return true;
        }









        static void PushBackHit(ref int force, ref PlayerControllerB playerWhoHit, ref EnemyAI __instance)
        {

            UnityEngine.Vector3 ePos = __instance.gameObject.transform.position;

            UnityEngine.Vector3 pPos = playerWhoHit.gameObject.transform.position;

            float verticalDistance = pPos.y - ePos.y;

            if (verticalDistance > 40f)
            {
                __instance.KillEnemy();
                return;
            }

            UnityEngine.Vector3 velocity = UnityEngine.Vector3.Normalize((pPos - ePos)* force);

            __instance.gameObject.transform.position += velocity;

        }








    }
}

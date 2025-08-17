using HarmonyLib;
using UnityEngine;

namespace GreyServers.HarmonyPatches
{
    [HarmonyPatch(typeof(GorillaScoreboardSpawner))]
    [HarmonyPatch("OnJoinedRoom")]
    internal class ScoreboardPatch
    {
        private static void Prefix(GorillaScoreboardSpawner __instance)
        {
            bool flag = __instance.notInRoomText == null;
            if (flag)
            {
                __instance.notInRoomText = new GameObject();
            }
        }
    }
}
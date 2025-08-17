using System;
using GorillaNetworking;
using HarmonyLib;
using PlayFab;

namespace GreyServers.HarmonyPatches
{
    [HarmonyPatch(typeof(GorillaComputer))]
    [HarmonyPatch("SwitchToLoadingState")]
    internal class ComputerLoadingPatch
    {
        private static bool Prefix()
        {
            bool flag = !PlayFabClientAPI.IsClientLoggedIn();
            return !flag;
        }
    }
}
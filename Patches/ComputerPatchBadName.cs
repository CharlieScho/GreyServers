using System;
using GorillaNetworking;
using HarmonyLib;
using PlayFab.CloudScriptModels;
using Photon.Pun;

namespace GreyServers.HarmonyPatches
{
    [HarmonyPatch(typeof(GorillaComputer))]
    [HarmonyPatch("ScreenStateExecution")]
    internal class ComputerPatchBadName
    {
        private static bool Prefix(string nameToCheck, bool forRoom, Action<ExecuteFunctionResult> resultCallback)
        {
            if (forRoom)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(nameToCheck, JoinType.Solo);
            }
            else
            {
                NetworkSystem.Instance.SetMyNickName(nameToCheck);
                GorillaComputer.instance.savedName = nameToCheck;
                GorillaComputer.instance.currentName = nameToCheck;
            }
            return false;
        }
    }
}
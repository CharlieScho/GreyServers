using System;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;

namespace GreyServers.HarmonyPatches
{
    [HarmonyPatch(typeof(GorillaComputer))]
    [HarmonyPatch("ScreenStateExecution")]
    internal class GorillaComputerScreenPatch
    {
        private static void Postfix()
        {
            string str = PhotonNetwork.CountOfPlayers.ToString();
            bool flag = GorillaComputer.instance.currentState == GorillaComputer.ComputerState.Startup;
            if (flag)
            {
                GorillaComputer.instance.screenText.Set("UNIXITY OS\n\n" + str + " PLAYERS ONLINE\n\n0 USERS BANNED YESTERDAY\n\nPRESS ANY KEY TO BEGIN");
            }
        }
    }
}
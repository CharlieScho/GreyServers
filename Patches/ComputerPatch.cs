using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;

[HarmonyPatch(typeof(GorillaComputer))]
[HarmonyPatch("GeneralFailureMessage")]
internal class ComputerPatch
{
    // Token: 0x0600004F RID: 79 RVA: 0x00003DDC File Offset: 0x00001FDC
    private static bool Prefix(string failMessage)
    {
        bool flag = failMessage.Contains("BANNED");
        bool result;
        if (flag)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = "7ef2fd75-edeb-4e41-9af1-cff0e7f07b3f";
            PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = "16818aa8-e19f-4cb0-a8ce-4a1fdf0ea451";
            PhotonNetwork.ConnectUsingSettings();
            result = false;
        }
        else
        {
            result = true;
        }
        return result;
    }
}
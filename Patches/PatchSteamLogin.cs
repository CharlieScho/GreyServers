using GorillaNetworking;
using HarmonyLib;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static GorillaNetworking.PlayFabAuthenticator;

namespace GreyServers.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork))]
    [HarmonyPatch("ConnectUsingSettings")]
    [HarmonyPatch(new Type[] { typeof(AppSettings), typeof(bool) })]
    class Patch_PhotonNetwork_Connect
    {
        static void Prefix(ref AppSettings appSettings)
        {
            Debug.Log("setting appid");
            appSettings.AppIdRealtime = "7cd85025-b15f-4d50-9247-59c978b147d9";
            appSettings.AppIdVoice = "648aa6ae-c571-49ca-9994-89ef43f4d47d";
        }
    }

    [HarmonyPatch(typeof(NetworkSystemPUN), "SetAuthenticationValues")]
    class Patch_SetAuthenticationValues_ForceNone
    {
        static void Prefix(AuthenticationValues authValues)
        {
            if (authValues == null) return;
            authValues.AuthType = CustomAuthenticationType.None;
            PhotonNetwork.AuthValues = authValues;
            Debug.Log("set authtype to none");
            return;
        }
    }

    [HarmonyPatch(typeof(PhotonAuthenticator), "SetCustomAuthenticationParameters")]
    class Patch_AuthenticationParameters
    {
        static void Prefix(Dictionary<string, object> customAuthData)
        {
            AuthenticationValues authenticationValues = new AuthenticationValues();
            authenticationValues.AuthType = CustomAuthenticationType.None;
            authenticationValues.SetAuthPostData(customAuthData);
            NetworkSystem.Instance.SetAuthenticationValues(authenticationValues);
            Debug.Log("[GreyServers] Set Photon auth data. AppVersion is: " + NetworkSystemConfig.AppVersion);
            return;
        }
    }

    [HarmonyPatch(typeof(PlayFabAuthenticator), "AuthenticateWithPlayFab")]
    public static class PlayFabAuthPatch
    {
        private static bool Prefix(PlayFabAuthenticator __instance)
        {
            PlayFabSettings.staticSettings.TitleId = "1457F3";

            string customId = PlayerPrefs.GetString("PlayFabCustomId", "");
            if (string.IsNullOrEmpty(customId))
            {
                customId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString("PlayFabCustomId", customId);
                PlayerPrefs.Save();
            }

            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
            {
                CustomId = customId,
                CreateAccount = true
            },
            (result) => {
                __instance.StartCoroutine(AdvanceLoginWithPlayFabData(__instance, result));
            },
            (error) => {
                Debug.LogError("Error logging into custom PlayFab title: " + error.GenerateErrorReport());
            });

            return false;
        }

        private static IEnumerator AdvanceLoginWithPlayFabData(PlayFabAuthenticator instance, LoginResult result)
        {
            var playFabPlayerIdField = AccessTools.Field(typeof(PlayFabAuthenticator), "_playFabPlayerIdCache");
            var sessionTicketField = AccessTools.Field(typeof(PlayFabAuthenticator), "_sessionTicket");

            playFabPlayerIdField.SetValue(instance, result.PlayFabId);
            sessionTicketField.SetValue(instance, result.SessionTicket);

            var advanceLoginMethod = AccessTools.Method(typeof(PlayFabAuthenticator), "AdvanceLogin");
            advanceLoginMethod.Invoke(instance, null);

            yield return null;
        }
    }

    [HarmonyPatch(typeof(GorillaServer), "GetAcceptedAgreements")]
    public static class Patch_GetAcceptedAgreements
    {
        private static bool Prefix(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback)
        {
            Dictionary<string, string> acceptedAgreements = new Dictionary<string, string>();

            if (request.AgreementKeys != null)
            {
                foreach (string key in request.AgreementKeys)
                {
                    acceptedAgreements[key] = "accepted";
                }
            }

            if (successCallback != null)
            {
                successCallback(acceptedAgreements);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GorillaServer), "AddOrRemoveDLCOwnership")]
    public static class Patch_ForceDLCOwnershipLegacy
    {
        private static bool Prefix(GorillaServer __instance, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest
            {
                FunctionName = "AddOrRemoveDLCOwnershipV2",
                FunctionParameter = new { }
            }, delegate (PlayFab.ClientModels.ExecuteCloudScriptResult result)
            {
                try
                {
                    ExecuteFunctionResult executeFunctionResult = new ExecuteFunctionResult();
                    executeFunctionResult.FunctionName = result.FunctionName;
                    executeFunctionResult.FunctionResult = result.FunctionResult;
                    executeFunctionResult.Error = null;

                    if (successCallback != null)
                    {
                        successCallback(executeFunctionResult);
                    }
                }
                catch (Exception ex)
                {
                    if (errorCallback != null)
                    {
                        errorCallback(new PlayFabError { ErrorMessage = "Error converting ExecuteCloudScriptResult: " + ex.Message });
                    }
                }
            }, errorCallback, null, null);
            return false;
        }
    }
}

using BepInEx;
using GorillaNetworking;
using GreyServers.HarmonyPatches;
using Photon.Pun;
using PlayFab;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace GreyServers
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Debug.Log("current title id is " + PlayFabSettings.TitleId);
            PlayFabSettings.TitleId = "1457F3";
            PlayFabAuthenticatorSettings.TitleId = "1457F3";
            HarmonyPatches.HarmonyPatches.ApplyHarmonyPatches();
            Debug.Log("setting titleid to " + PlayFabSettings.TitleId);
        }

        private void Start()
        {
            Debug.Log(PlayFabAuthenticatorSettings.AuthApiBaseUrl);
            UnityEngine.Object.DontDestroyOnLoad(this);
            GameObject target = new GameObject();
        }
    }
}

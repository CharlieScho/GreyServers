using System;
using BepInEx;
using UnityEngine;
using Photon.Pun;
using GreyServers.HarmonyPatches;

namespace GreyServers
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	public class Plugin : BaseUnityPlugin
	{
		private void Start()
		{
            UnityEngine.Object.DontDestroyOnLoad(this);
            GameObject target = new GameObject();
            HarmonyPatches.HarmonyPatches.ApplyHarmonyPatches();
        }
    }
}

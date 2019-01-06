// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.
#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MPG.SocialPlatforms
{
	public class SteamworksUtility : MonoBehaviour
	{
		private static SteamworksUtility instance;

		public static SteamworksUtility Instance
		{
			get
			{
				if (instance == null)
				{
					GameObject go = new GameObject("SteamCoroutineRunner");
					instance = go.AddComponent<SteamworksUtility>();
				}
				
				return instance;
			}
		}

		private void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}
}
#endif //!DISABLESTEAMWORKS
// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.
#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	internal class SteamworksUser : SocialUserBase<SteamworksPlatform>
	{
		public SteamworksUser(SteamworksPlatform platform) : base(platform) {}
		public SteamworksUser() : base()
		{
			base.platform = SteamworksPlatform.Instance;
		}
	}
}
#endif //!DISABLESTEAMWORKS
// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.
#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MPG.SocialPlatforms
{
	public static class SteamworksPlatformConfig
	{
		/// <summary>
		/// Provides a name for the leaderboard that is used in a friends listing mechanism
		/// </summary>
		internal static string steamLeaderBoardName = Application.productName + " Friends";

		/// <summary>
		/// Platform specific initialization and activate SteamworksPlatform as a Social.Active
		/// </summary>
		/// <param name="getAchievementName">the achievements names corresponding to their numeric IDs</param>
		/// <param name="getStatName">the stat names corresponding to their numeric IDs</param>
		/// <param name="logger">logger instance, if you want to override a default ouput to Debug.LogXXX output</param>
		public static void Initialize(Func<int, string> getAchievementName = null, Func<int, string> getStatName = null, ILogger logger = null)
		{
			Log = (logger == null ? 
				new LoggerImpl() : //default logger
				logger);
			
			GetAchievementName = (getAchievementName == null ?
				(index) => { return string.Format("{a{0:D3}}", index); } : //default achievement name format
				getAchievementName);

			GetAchievementId = GetAchievementName;

			GetStatName = (getStatName == null ?
				(index) => { return string.Format("{s{0:D3}}", index); } : //default stat name format
				getStatName);

			//Initialize Unity social platform with Steam platform. Steam client must be already running.
			SteamworksPlatform.Initialize(new GameObject("SteamPlatform"));
		}

		/// <summary>
		/// Provides an achievement name by its index.
		/// </summary>
		/// <param name="i">achievement index</param>
		/// <returns></returns>
		internal static System.Func<int, string> GetAchievementName;

		/// <summary>
		/// Provides a stat name by it's index.
		/// </summary>
		/// <param name="i">stat index</param>
		/// <returns></returns>
		internal static System.Func<int, string> GetStatName;

		/// <summary>
		/// Provides an achievement ID by it's index for internal usage. 
		/// </summary>
		/// <param name="i">achievement ID index</param>
		/// <returns>achievement Id for the internal collection</returns>
		internal static System.Func<int, string> GetAchievementId;

		/// <summary>
		/// Logger instance
		/// </summary>
		internal static ILogger Log
		{
			get { return SocialUtils.Log; }
			set { SocialUtils.Log = value; }
		}

		/// <summary>
		/// Steam application ID
		/// </summary>
		public static string AppID { get { return Steamworks.SteamUtils.GetAppID().ToString();  } }
	}
}
#endif //!DISABLESTEAMWORKS
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
	public class SteamworksAchievement : IAchievement, IAchievementDescription
	{
		internal bool isIntValue = false;
		public SteamworksAchievement()	{}
		public string achievedDescription { get; set; }
		public bool completed  { get; set; }
		public bool hidden { get; set; }
		public string id { get; set; }
		public string statId { get; set; }
		public Texture2D image { get; set; }
		public DateTime lastReportedDate { get; set; }
		public double percentCompleted { get; set; }
		public int points { get { return (int)this.percentCompleted;  } }
		public string title { get; set; }
		public string unachievedDescription { get; set; }
		public void ReportProgress(Action<bool> callback)
		{
			SteamworksPlatform.Instance.ReportProgress(id, isIntValue? points : percentCompleted, callback);
		}
	}
}
#endif //!DISABLESTEAMWORKS
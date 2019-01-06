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
	/// <summary>
	/// Not implemented yet
	/// </summary>
	public class SteamworksLeaderboard : ILeaderboard
	{
		public string id
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool loading
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IScore localUserScore
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public uint maxRange
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Range range
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public IScore[] scores
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public TimeScope timeScope
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public string title
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public UserScope userScope
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public void LoadScores(Action<bool> callback)
		{
			throw new NotImplementedException();
		}

		public void SetUserFilter(string[] userIDs)
		{
			throw new NotImplementedException();
		}
	}
}
#endif //!DISABLESTEAMWORKS
// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

using System;
using System.Collections;
using System.Collections.Generic;
using MPG.SocialPlatforms;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	public abstract class PlatformBase : MonoBehaviour
	{
		internal static void Initialize<TPlatform>(GameObject go) where TPlatform : Component
		{
			if(Social.Active != null && Social.Active is TPlatform)
				return;

			TPlatform platform = go.GetComponent<TPlatform>();
			if(platform == null)
				platform = go.AddComponent<TPlatform>();

			Social.Active = platform as ISocialPlatform;
			SocialUtils.Log.message("PlatformBase.Initialize: Platform: {0} activated as a Social platform", typeof(TPlatform));
		}
	}

	internal class SocialPlatformBase<TUser, TAchievement, TLeaderboard> : PlatformBase
		where TUser: ILocalUser, new()
		where TAchievement: IAchievement, new()
		where TLeaderboard: ILeaderboard, new()
	{
		protected static SocialPlatformBase<TUser, TAchievement, TLeaderboard> instance;
		protected TUser user;
#pragma warning disable 649
		protected IUserProfile[] friendsAppUsers;
#pragma warning restore 649

		// Use this for initialization
		protected void Awake()
		{
			instance = this;
			user = new TUser();
		}

		public IUserProfile[] Friends
		{
			get
			{
				return friendsAppUsers;
			}
		}

		public ILocalUser localUser
		{
			get
			{
				return user;
			}
		}

		/// <summary>
		/// this is only to make compiler happy - shouldn't ever been called
		/// </summary>
		private void Authenticate(ILocalUser user, Action<bool, string> callback)
		{
			throw new NotImplementedException();
		}

		public void Authenticate(ILocalUser user, Action<bool> callback)
		{
			Authenticate(user, (isSuccess, message) => callback(isSuccess));
		}

		public IAchievement CreateAchievement()
		{
			return new TAchievement();
		}

		public ILeaderboard CreateLeaderboard()
		{
			return new TLeaderboard();
		}

		public bool GetLoading(ILeaderboard board)
		{
			return false;
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			if (callback != null)
				callback(new IAchievementDescription[] { });
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			if (callback != null)
				callback(new IAchievement[] { });
		}

		public void LoadScores(ILeaderboard board, Action<bool> callback)
		{
			if (callback != null)
				callback(false);
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			if (callback != null)
				callback(new IScore[] { });
		}

		public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			if (callback != null)
				callback(new IUserProfile2[] { });
		}

		public void ReportProgress(string achievementID, double progress, Action<bool> callback)
		{
			if (callback != null)
				callback(false);
		}

		public void ReportScore(long score, string board, Action<bool> callback)
		{
			if (callback != null)
				callback(false);
		}

		public void ShowAchievementsUI()
		{
			//throw new NotImplementedException();
		}

		public void ShowLeaderboardUI()
		{
			//throw new NotImplementedException();
		}
	}
}
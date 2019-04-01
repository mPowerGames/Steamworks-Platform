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
using Steamworks;

namespace MPG.SocialPlatforms
{
	using TBase = SocialPlatformBase<SteamworksUser, SteamworksAchievement, SteamworksLeaderboard>;

	internal class SteamworksPlatform : TBase, ISocialPlatform2
	{
#pragma warning disable 414
		private Callback<GameOverlayActivated_t> gameOverlayActivated;
		private Callback<GetAuthSessionTicketResponse_t> getAuthSessionTicketResponse;

		private Callback<UserStatsReceived_t> userStatsReceived;
		private Callback<UserStatsStored_t> userStatsStored;
		private Callback<UserAchievementStored_t> userAchievementStored;

		private CallResult<LeaderboardFindResult_t> leaderboardFindResult;
		private CallResult<LeaderboardScoresDownloaded_t> leaderboardScoresDownloaded;
		private CallResult<LeaderboardScoreUploaded_t> leaderboardScoreUploaded;
#pragma warning restore 414
		private byte[] ticket;
		private bool isGettingFriendsPaged;
		private bool isGettingFriendsPagedFinished = true; //yes it is ugly :(

		private CGameID gameID;
		private bool isAuthenticated;
		private string authSessionTicket; //This will have a value in case of successful initialization
		int lastUpdatedIndex = 0;
		bool isRetryingFindLeaderboard = false;

		SteamLeaderboard_t leaderboard;
		CSteamID[] rawFriends;
		List<IUserProfile> friends;
		SteamworksAchievement[] achievements;

		Action<bool> delayedFriendsLoadedCallback;
		Action<IAchievement[]> achLoadCallback;
		Action<IAchievementDescription[]> achDescLoadCallback;
		Action<bool, string> authCallback;
		Action<bool> achReportedCallback;

		/// <summary>
		/// May be used to access some extended functionality, that is not supported by Unity social interfaces
		/// </summary>
		public static SteamworksPlatform Instance { get { return (SteamworksPlatform)instance; } }

		/// <summary>
		/// Should be called prior to everything else, to initialize the platform
		/// </summary>
		/// <param name="go"></param>
		public static void Initialize(GameObject go)
		{
			TBase.Initialize<SteamworksPlatform>(go);
			if (SteamManager.Initialized)
			{
				SteamUtils.SetOverlayNotificationPosition(ENotificationPosition.k_EPositionTopLeft);
				Instance.gameID = new CGameID(SteamUtils.GetAppID());
			}

			Instance.isGettingFriendsPaged = false;
			Instance.isAuthenticated = false;
		}

		/// <summary>
		/// After the successful friends list request returns an array of friend data
		/// </summary>
		public new IUserProfile[] Friends
		{
			get
			{
				return friends.ToArray();
			}
		}

		/// <summary>
		/// The authentication result
		/// </summary>
		public bool IsAuthenticated
		{
			get
			{
				return isAuthenticated;
			}
		}

		/// <summary>
		/// Auth session ticket. Useful for client server authorization.
		/// </summary>
		public string AuthSessionTicket
		{
			get
			{
				return authSessionTicket;
			}
		}

		/// <summary>
		/// Steam callbacks management. Not intended to be called directly.
		/// </summary>
		public void OnEnable()
		{
			if (SteamManager.Initialized)
			{
				gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
				getAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);

				userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
				userStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
				userAchievementStored = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);

				leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
				leaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
				leaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);

				//SteamworksPlatformConfig.Log.message("SteamPlatform: callbacks initialized");
			}
		}

		/// <summary>
		/// This callback is called by Steam when the session ticket is received
		/// </summary>
		/// <param name="param">holds the result of the ticket request</param>
		private void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t param)
		{
			//SocialUtils.Log.message("SteamworksPlatform.OnGetAuthSessionTicketResponse: result: " + param.m_eResult + " ticket: " + param.m_hAuthTicket);
			switch (param.m_eResult)
			{
				case EResult.k_EResultOK:
					isAuthenticated = true;
					if (ticket != null)
					{
						System.Text.StringBuilder hexTicket = new System.Text.StringBuilder();
						foreach (byte b in ticket)
						{
							hexTicket.Append(b.ToString("X2"));
						}
						//ticket
						SteamworksPlatformConfig.Log.message("SteamworksPlatform.Auth Ticket: {0}", hexTicket.ToString());
						authSessionTicket = hexTicket.ToString();

						user.id = SteamUser.GetSteamID().ToString().Clone().ToString();
						user.userName = SteamFriends.GetPersonaName().Clone().ToString();

						authCallback(true, "Success");
						authCallback = null;
					}
					else
					{
						authCallback(false, "Error authenticating - access token is empty");
						authCallback = null;
						return;
					}

					break;
				default:
					SteamworksPlatformConfig.Log.error("SteamPlatform.OnGetAuthSessionTicketResponse: result" + param.m_eResult);

					isAuthenticated = false;
					authCallback(false, param.m_eResult.ToString());
					authCallback = null;
					break;
			}
		}

		/// <summary>
		/// This method is a way to login to get a session ticket for the client-server authorization
		/// </summary>
		/// <param name="user"></param>
		/// <param name="callback"></param>
		public void Authenticate(ILocalUser user, Action<bool, string> callback)
		{
			if (!SteamManager.Initialized)
			{
				callback(false, "Steam is not initialized");
				return;
			}

			if(!AuthImpl())
			{
				callback(false, "Steam is not initialized");;
				return;
			}

			authCallback = callback; //delaying a callback launch till authentication callback
		}

		/// <summary>
		/// Implementation of the authorization routine 
		/// </summary>
		/// <returns></returns>
		bool AuthImpl()
		{
			isAuthenticated = false;
			if (!SteamManager.Initialized)
			{
				return false;
			}

			uint pcbTicket;
			ticket = new byte[1024];

			SteamUser.GetAuthSessionTicket(ticket, 1024, out pcbTicket);
			return true;
		}

		private void OnGameOverlayActivated(GameOverlayActivated_t param)
		{
			SteamworksPlatformConfig.Log.message("SteamworksPlatform.OnGameOverlayActivated: " + param.m_bActive);
		}

		/// <summary>
		/// Similar to <seealso cref="LoadAchievements(Action{IAchievement[]})"/> except for the callback parameter.
		/// </summary>
		/// <param name="callback"></param>
		public new void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			achDescLoadCallback = callback;
			SteamUserStats.RequestCurrentStats();
		}

		/// <summary>
		/// Requests (asynchronously) the achievements list from Steam. 
		/// </summary>
		/// <param name="callback"></param>
		public new void LoadAchievements(Action<IAchievement[]> callback)
		{
			if (!SteamManager.Initialized)
			{
				return;
			}

			achLoadCallback = callback;
			SteamUserStats.RequestCurrentStats();
		}

		void OnUserStatsReceived (UserStatsReceived_t param)
		{
			if(param.m_nGameID != (ulong)gameID)
				return;
			
			//Log.warning("SteamworksPlatform.OnUserStatsReceived: eResult = " + param.m_eResult + " nGameID = " + 
			//			param.m_nGameID + " steamIDUser = " + param.m_steamIDUser);
		
			uint achivementsCount = SteamUserStats.GetNumAchievements();
			int val = 0;
			float dVal = 0;
            string statName = string.Empty;
			string achName = string.Empty;
			bool isAchieved;
			
			achievements = new SteamworksAchievement[achivementsCount];

			for(int i = 0; i < achivementsCount; ++i)
			{
				SteamworksAchievement sa = new SteamworksAchievement();
				statName = SteamworksPlatformConfig.GetStatName(i);
				achName = SteamworksPlatformConfig.GetAchievementName(i);

				if (string.IsNullOrEmpty(achName) || string.IsNullOrEmpty(statName)) //temporary workaround for the lack of float stats support
					continue;

				if (SteamUserStats.GetStat(statName, out val))
				{
					sa.isIntValue = true;
					sa.percentCompleted = val;
				}
				else if (SteamUserStats.GetStat(statName, out dVal))
				{
					sa.isIntValue = false;
					sa.percentCompleted = dVal;
				}
				else
				{
					SteamworksPlatformConfig.Log.error("SteamworksPlatform: Error getting stat: " + statName);
					//continue;
				}
			
				if(SteamUserStats.GetAchievement(achName, out isAchieved))
					sa.completed = isAchieved;
				else
				{
					SteamworksPlatformConfig.Log.error("SteamworksPlatform: Error getting achievement state: " + achName);
					//continue;
				}

				sa.title = SteamUserStats.GetAchievementDisplayAttribute(achName, "name");
				sa.achievedDescription = SteamUserStats.GetAchievementDisplayAttribute(achName, "desc");
				sa.hidden = SteamUserStats.GetAchievementDisplayAttribute(achName, "hidden") == "1" ? true : false;
				sa.id = SteamworksPlatformConfig.GetAchievementId(i);
				sa.statId = statName;

				//SteamworksPlatformConfig.Log.message("STAT: " + statName + " = " + val + " isAchieved: " + isAchieved);
				achievements[i] = sa;
			}

			if(achDescLoadCallback != null)
				achDescLoadCallback(achievements);
			if(achLoadCallback != null)
				achLoadCallback(achievements);
		}

		/// <summary>
		/// Sends the achievement progress to Steam
		/// </summary>
		/// <param name="achievementID"></param>
		/// <param name="progress">points only! not %! i.e. 1, 2, ... 150K, ...</param>
		/// <param name="callback"></param>
		public new void ReportProgress(string achievementID, double progress, Action<bool> callback)
		{
			SteamworksAchievement sa = getAchievement(achievementID);

			if (sa == null)
			{
				SteamworksPlatformConfig.Log.error ("Achievements are not initialized, but we're trying to send data for: " + achievementID);
				if(callback != null)
					callback(false);
                return; //can't set achievements, when not initilized data
			}

			if (sa.completed)
			{
				if (callback != null)
					callback(true);
				return;
			}

			SteamworksPlatformConfig.Log.message("Reporting achievement: id={0}, old int val={1} old float val={2}", achievementID, sa.points, sa.percentCompleted);
			if (sa.percentCompleted > progress)
			{
				SteamworksPlatformConfig.Log.warning("Achievement progress is higher than the reported one for: " + achievementID);
				if (callback != null)
					callback(false);
				return;
			}

			//We use either int or float value depending on the flag in the achievement, that we received during the platform initialization
			if (sa.isIntValue)
			{
				if (SteamUserStats.SetStat(sa.statId, (int)progress))
				{
					SteamworksPlatformConfig.Log.message("Successfully set an achievement progress marked as INT: " + sa.id);
					sa.percentCompleted = progress;
				}
				else
				{
					SteamworksPlatformConfig.Log.error("Unable to set an achievement progress marked as INT: " + sa.id);
					callback(false);
					return;
				}
			}
			else if (SteamUserStats.SetStat(sa.statId, (float)progress))
			{
				SteamworksPlatformConfig.Log.message("Successfully set an achievement progress marked as FLOAT: " + sa.id);
				sa.percentCompleted = progress;
			}
			else
			{
				SteamworksPlatformConfig.Log.error("Unable to set an achievement progress marked as FLOAT: " + sa.id);
				callback(false);
				return;
			}
			
			achReportedCallback = callback; //this will be overwritten in case of several sequential calls
			SteamUserStats.StoreStats(); //this is slow but async
		}

		SteamworksAchievement getAchievement(string achievementId)
		{
			if(achievements == null || achievements.Length == 0)
				return null;

			try
			{
				return Array.Find(achievements, (ach) => { return ach.id == achievementId; });
			}
			catch
			{
				SteamworksPlatformConfig.Log.error("Problem with internal achievement lookup for: " + achievementId);
				throw;
			}
		}

		void OnUserStatsStored (UserStatsStored_t param)
		{
			if(param.m_nGameID != (uint)gameID)
				return;
			
			if(param.m_eResult != EResult.k_EResultOK)
			{
				SteamworksPlatformConfig.Log.error("SteamworksPlatform.OnUserStatsStored: eResult = " + param.m_eResult );
				SteamUserStats.StoreStats(); // retrying

				if (achReportedCallback != null)
					achReportedCallback(false);
				return;
			}

			if (achReportedCallback != null)
				achReportedCallback(true);
			SteamworksPlatformConfig.Log.message("SteamworksPlatform.OnUserStatsStored: eResult = " + param.m_eResult + " nGameID = " + param.m_nGameID);
		}

		//I doubt we will ever need it, but for now let it be here
		void OnUserAchievementStored (UserAchievementStored_t param)
		{
			if(param.m_nGameID != (uint)gameID)
				return;

			SteamworksPlatformConfig.Log.message("SteamworksPlatform.OnUserAchievementStored: " +
						" bGroupAchievement = " + param.m_bGroupAchievement +
						" nCurProgress = " + param.m_nCurProgress +
						" nGameID = " + param.m_nGameID +
						" nMaxProgress = " + param.m_nMaxProgress +
						" rgchAchievementName = " + param.m_rgchAchievementName);

			//SteamUserStats.IndicateAchievementProgress(param.m_rgchAchievementName, param.m_nCurProgress, param.m_nMaxProgress);
		}

		/// <summary>
		/// Requests user friends list using a leaderbord trick, as there's no simple way to get a friend list in Steam. After the callback is fired,
		/// the <see cref="Friends"/> will be available.
		/// </summary>
		/// <param name="user">user who's friends are to be acquired</param>
		/// <param name="callback">result callback</param>
		public void LoadFriends(ILocalUser user, Action<bool> callback)
		{
			if (!SteamManager.Initialized)
			{
				callback(false);
				return;
			}

			delayedFriendsLoadedCallback = callback;
			//initialization for friend finder mechanism
			SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(SteamworksPlatformConfig.steamLeaderBoardName, 
				ELeaderboardSortMethod.k_ELeaderboardSortMethodAscending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNone);
			leaderboardFindResult.Set(handle);
			//SocialUtils.Log.message("SteamworksPlatform.LoadFriends: Requested friends via a leaderboard");
		}

		void OnLeaderboardFindResult(LeaderboardFindResult_t param, bool bIOFailure)
		{
			SteamAPICall_t handle;
			if(bIOFailure)
			{
				if(isRetryingFindLeaderboard)
					delayedFriendsLoadedCallback(false);
				else
				{
					isRetryingFindLeaderboard = true;
					handle = SteamUserStats.FindLeaderboard(SteamworksPlatformConfig.steamLeaderBoardName);
					leaderboardFindResult.Set(handle);
				}
				return;
			}
			else
			{
				isRetryingFindLeaderboard = false;
			}

			leaderboard = param.m_hSteamLeaderboard;
			handle = SteamUserStats.UploadLeaderboardScore(leaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, 1, new int[] { 1 }, 1);
			leaderboardScoreUploaded.Set(handle);
			SteamworksPlatformConfig.Log.message("SteamworksPlatform.OnLeaderboardFindResult: Uploading leaderboard score");
		}

		void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t param, bool bIOFailure)
		{
			if(bIOFailure)
			{
				delayedFriendsLoadedCallback(false);
				return;
			}

			int friendsCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
			SteamworksPlatformConfig.Log.message("Friends count: " + friendsCount);
		
			rawFriends = new CSteamID[friendsCount];
			for(int i = 0; i < friendsCount; ++i)
			{
				rawFriends[i] = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
			}
		
			SteamworksPlatformConfig.Log.message("SteamWorkshopController.Raw friends list created");
			SteamworksUtility.Instance.StartCoroutine(requestFriendsPaged());
		}

		IEnumerator requestFriendsPaged()
		{
			if(rawFriends.Length > 100)
			{
				int delta = 100;		
				int currentPosition = 0;
				
				while(rawFriends.Length - currentPosition > 0)
				{
					isGettingFriendsPaged = true;
					isGettingFriendsPagedFinished = false;

					CSteamID[] currentFriendsSet = new CSteamID[delta];
					System.Array.Copy(rawFriends, currentPosition, currentFriendsSet, 0, delta);
				
					SteamworksPlatformConfig.Log.message("SteamworksPlatform.requestFriendsPaged: paged friends leaderboards request: cur.pos = " + currentPosition + " delta = " + delta + " total = " + rawFriends.Length);
					SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboard, currentFriendsSet, currentFriendsSet.Length);
					leaderboardScoresDownloaded.Set(handle);
				
					bool isFailed = false;
					while(!SteamUtils.IsAPICallCompleted(handle, out isFailed))
					{
						yield return new WaitForSeconds(0.1f);
					}
				
					//wait to make sure our callback has already been called
					yield return new WaitForSeconds(0.1f);
				
					if(!isFailed)
					{
						currentPosition += delta;
						if(rawFriends.Length - currentPosition < 100)
							delta = rawFriends.Length - currentPosition;
						}
					else
					{
						SteamworksPlatformConfig.Log.error("SteamworksPlatform.DownloadLeaderboardEntriesForUsers: failed at pos: " + currentPosition + " out of " + rawFriends.Length + " delta: " + delta + "\nRetrying...");
						yield return new WaitForSeconds(1f);
					}
				}
				isGettingFriendsPagedFinished = true;
			}
			else
			{
				isGettingFriendsPaged = false;
				isGettingFriendsPagedFinished = true;
				SteamworksPlatformConfig.Log.message("SteamworksPlatform.requestFriendsPaged: non-paged friends leaderboards request");
				SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboard, rawFriends, rawFriends.Length);
				leaderboardScoresDownloaded.Set(handle);
			}
		
		}

		//a cheater way to get app users
		void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool bIOFailure)
		{
			//Log.warning ("SteamworksPlatform.OnLeaderboardScoresDownloaded");	
			if (friends == null)
				friends = new List<IUserProfile>();
			else if (!isGettingFriendsPaged)
				friends.Clear();

			CSteamID fSteamId;
			LeaderboardEntry_t leaderboardEntry;
			int[] details = new int[1] { 0 };

			for (int i = lastUpdatedIndex; i < param.m_cEntryCount; ++i)
			{
				SteamUserStats.GetDownloadedLeaderboardEntry(param.m_hSteamLeaderboardEntries, i, out leaderboardEntry, details, 1);
				fSteamId = leaderboardEntry.m_steamIDUser;
				
				SteamworksUserProfile friend = new SteamworksUserProfile(fSteamId, 
					SteamFriends.GetFriendPersonaName(fSteamId).Clone().ToString());
				friends.Add(friend as IUserProfile);
			}

			if(!isGettingFriendsPaged || isGettingFriendsPagedFinished)
				delayedFriendsLoadedCallback(true);
		}

		/// <summary>
		/// Opens a Steam overlay to user with a chat window to his friend
		/// </summary>
		/// <param name="user"></param>
		public void ShowInviteToGame(SteamworksUserProfile user)
		{
			if(user == null)
				SteamFriends.ActivateGameOverlay("Friends");
			else
				SteamFriends.ActivateGameOverlayToUser("chat", user.SteamID);
		}
		
		/// <summary>
		/// Reset all game achievements in Steam (for testing purpose).
		/// Note: in a case of error "SteamworksPlatform.OnUserStatsStored: eResult = k_EResultInvalidParam"
		/// check your Stats constraints settings (Increment Only?, Max Change, Min Change, Max Value)
		/// </summary>
		public void ResetAllAchievements()
		{
			if (!SteamManager.Initialized)
			{
				return;
			}
			
			SteamUserStats.ResetAllStats(true);
			SteamUserStats.RequestCurrentStats();
		}
	}
}
#endif //!DISABLESTEAMWORKS

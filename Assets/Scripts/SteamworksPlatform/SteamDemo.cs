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
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class SteamDemo : MonoBehaviour {
	public Text textOutput;

	/// <summary>
	/// Achievement names array
	/// </summary>
	static string[] achievementNames = new string[] { "ACH_WIN_ONE_GAME", "ACH_WIN_100_GAMES", "ACH_TRAVEL_FAR_ACCUM", "ACH_TRAVEL_FAR_SINGLE"};
	/// <summary>
	/// Stats used to count the achievement progress.
	/// </summary>
	static string[] stats = new string[] { "NumGames", "NumWins", "NumLosses", "FeetTraveled", "AverageSpeed", "Unused2", "MaxFeetTraveled" };

	private IAchievement[] achievements;

	void Start () {
		//Initialization before anything else
		MPG.SocialPlatforms.SteamworksPlatformConfig.Initialize(
			(i) => achievementNames[i], //delegates are used to allow not only arrays but also things like: string.Format("{a{0:D3}}", index);
			(i) => stats [i]);

		writeLine("Steam Platform is initialized for app ID: " + MPG.SocialPlatforms.SteamworksPlatformConfig.AppID);

		//User authentication is necessary to have features like localUser name, picture, friends, auth.ticket
		Social.Active.Authenticate(null, (isSucceeded, errorMessage) => {
			writeLine("Steam is authenticated");
			writeLine("Local user name: " + Social.localUser.userName);
        });
    }
	
	public void OnAchievementsLoadButtonClick()
	{
		// Achievements must be loaded prior to reporting their progress. Use the acquired IAchievement[] array insted of manually creating achievements
		Social.Active.LoadAchievements((loadedAchievements) => {
			achievements = loadedAchievements;
			writeLine("Loaded achievements count: " + loadedAchievements.Length);

			foreach (var achievement in loadedAchievements)
			{
				writeLine("Achievement ID: " + achievement.id + " -> " + achievement.percentCompleted);
				writeLine("\t\t\t: " + " -> " + (achievement as IAchievementDescription).points);
			}
		});
	}

	void reportAchievement(int id)
	{
		/// Reporting achievements with IAchievement.completed = true is not supported at the moment
		/// Use IAchievement.percentCompleted = 100.0 or Social.Active.ReportProgress(id, 100.0, ...) or any other value that you've configured as a goal 
		/// for the corresponding achievement.
		IAchievement ach = achievements[id]; //int achievement value (common scenario)
		Social.Active.ReportProgress(ach.id, ach.percentCompleted + 1, (isSucceeded) => {
			writeLine("Reported achievement progress: " + ach.id + (isSucceeded ? " - Successfully" : " - Failure"));
		});
	}

	public void OnAchievement1ReportButtonClick()
	{
		reportAchievement(0);
    }

	public void OnAchievement2ReportButtonClick()
	{
		reportAchievement(1);
    }

	public void OnAchievement3ReportButtonClick()
	{
		reportAchievement(2);
    }

	public void OnAchievement4ReportButtonClick()
	{
		reportAchievement(3);
    }

	public void OnFriendsLoadButtonClick()
	{
		/// Loads a list of friends that are playing this game. This will be non-empty only after anyone of your friends on 
		/// Steam, will run this code (in the same game).
		Social.Active.LoadFriends(Social.Active.localUser, (isFriendListReady) => {
			writeLine("Loaded Steam friends count (for current game): " + Social.localUser.friends.Length);
			foreach (var friend in Social.localUser.friends)
			{
				writeLine("\t" + friend.userName);
			}
		});
    }

	public void OnShowAuthSessionTicketButtonClieck()
	{
		//The auth.session ticket becomes available after a successful call to Social.Active.Authenticate
		writeLine("Auth session ticket: " + MPG.SocialPlatforms.SteamworksPlatform.Instance.AuthSessionTicket);
	}

	void writeLine(string msg)
	{
		var rt = textOutput.GetComponent<RectTransform>();
		textOutput.text += msg + "\n";

		if(rt.sizeDelta.y < textOutput.preferredHeight)
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, textOutput.preferredHeight);
	}
}
#endif //!DISABLESTEAMWORKS
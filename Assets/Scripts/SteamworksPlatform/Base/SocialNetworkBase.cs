// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	public class SocialNetworkBase : MonoBehaviour
	{
		ISocialNetwork childObj;
		protected bool isInitilized;
		protected string lastInitError;
		protected string lastLoginError;

		protected void Start()
		{
			childObj = (ISocialNetwork)this;
		}

		public void Initialize<T>(Action<InitResultEventArgs> callback) where T: PlatformBase
		{
			//must reinitialize each time
			PlatformBase.Initialize<T>(gameObject);
			isInitilized = true;

			if(callback != null)
				callback(new InitResultEventArgs());
		}

		public void Login(Action<LoginResultEventArgs> callback)
		{
			SocialUtils.Log.message(childObj.ToString() + ": Login");
			if(Social.localUser.authenticated)
			{
				lastLoginError = string.Empty;
				callback(new LoginResultEventArgs());
				return;
			}

			Social.Active.Authenticate(null, delegate(bool result, string message)
			{
				SocialUtils.Log.message(childObj.ToString() + ".OnAuthenticate");

				if(!result)
				{
					lastLoginError = childObj.ToString();
					callback(new LoginResultEventArgs(message));
					return;
				}

				lastLoginError = string.Empty;
				SocialUtils.Log.message("{0}: User loged in acquired Name = {1}", childObj.ToString(), Social.localUser.userName);
				callback(new LoginResultEventArgs());
			});
		}

		public void GetAppUsers(Action<GetAppUsersEventArgs> callback)
		{
			if(!Social.localUser.authenticated)
			{
				callback(new GetAppUsersEventArgs(this.GetType().ToString() + ".GetAppUsers: localUser is not authenticated"));
				return;
			}

			Social.localUser.LoadFriends(delegate(bool result) {
				if(!result)
				{
					callback(new GetAppUsersEventArgs(this.GetType().ToString() + ".LoadFriends: Error loading friends"));
					return;
				}

				SocialUtils.Log.message("{0}.LoadFriends: succeeded, friends count = {1}", this.GetType().ToString(), Social.localUser.friends.Length);
				callback(new GetAppUsersEventArgs(Social.localUser.friends));
			});
		}

		#region Properties
		public bool Initialized
		{
			get { return isInitilized; }
		}

		public string LastInitError { get { return lastInitError; } }

		public string LastLoginError { get { return lastLoginError; } }

		public string UserName
		{
			get	{
				if(!Social.Active.localUser.authenticated)
				{
					SocialUtils.Log.message("{0}: User is null or non-authenticated", childObj.ToString());
					return string.Empty;
				}
				SocialUtils.Log.message("{0}: UserName = {1}", childObj.ToString(), Social.localUser.userName);
				return Social.localUser.userName;
			}
		}

		public Texture2D UserPhoto
		{
			get {
				if(!Social.localUser.authenticated)
				{
					SocialUtils.Log.error("{0}: User is null or non-authenticated", childObj.ToString());
					return Texture2D.whiteTexture;
				}

				//SocialUtils.Log.message("{0}: UserPhoto = {1}", childObj.ToString(), Social.localUser.image.name);
				return Social.localUser.image;
			}
		}

		public string UserId
		{
			get {
				if(!Social.localUser.authenticated)
				{
#if UNITY_EDITOR || DEBUG || TEST_SERVER
					SocialUtils.Log.error("{0}: User is null or non-authenticated", childObj.ToString());
#endif
					return string.Empty;
				}

				SocialUtils.Log.message("{0}: UserId = {1}", childObj.ToString(), Social.localUser.id);
				return Social.localUser.id;
			}
		}
		#endregion
	}
}
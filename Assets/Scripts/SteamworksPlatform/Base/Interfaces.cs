// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	/// <summary>
	/// This is a customizable interface for implementation in your platform controller, it provides a minimal set of features
	/// </summary>
	public interface ISocialNetwork
	{
		void Initialize(Action<InitResultEventArgs> callback);
		void GetAppUsers(Action<GetAppUsersEventArgs> callback);

		bool Initialized { get; }
		string AccessToken { get; }
		string UserId { get; }
		string UserName { get; }
		Texture2D UserPhoto { get; }
	};

	/// <summary>
	/// This interface is required for platforms that doesn't give us a URL for friends picture downloading
	/// </summary>
	internal interface IUserProfile2 : IUserProfile
	{
		string userImageUrl { get; }
	}

	/// <summary>
	/// This interface is required to be able to detect if the platform is authenticated (if it's not active) and get friends on this platform
	/// </summary>
	internal interface ISocialPlatform2 : ISocialPlatform
	{
		bool IsAuthenticated { get; }
		IUserProfile[] Friends { get; }
	}

	public interface ILogger
	{
		void message(string format, params object[] p);
		void warning(string format, params object[] p);
		void error(string format, params object[] p);
		void exception(System.Exception e);
	}
}
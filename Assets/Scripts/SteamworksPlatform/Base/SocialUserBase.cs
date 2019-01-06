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
	internal class SocialUserBase<TPlatform> : SocialUserProfileBase, ILocalUser
		where TPlatform: ISocialPlatform2
	{
		protected TPlatform platform;
		public SocialUserBase() : base(string.Empty, string.Empty, string.Empty) { }

		protected SocialUserBase(TPlatform platform) : base(string.Empty, string.Empty, string.Empty)
		{
			this.platform = platform;
		}

		public bool authenticated
		{
			get
			{
				return platform.IsAuthenticated;
			}
		}

		public IUserProfile[] friends
		{
			get
			{
				return platform.Friends;
			}
		}

		public bool underage
		{
			get
			{
				return false;
			}
		}

		public void Authenticate(Action<bool, string> callback)
		{
			platform.Authenticate(this, callback);
		}

		public void Authenticate(Action<bool> callback)
		{
			platform.Authenticate(this, callback);
		}

		public void LoadFriends(Action<bool> callback)
		{
			platform.LoadFriends(this, callback);
		}
	}
}
// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	internal  class SocialUserProfileBase : IUserProfile2
	{
		public SocialUserProfileBase(string userName, string userId, string imageUrl)
		{
			this.userName = userName;
			this.id = userId;
			this.userImageUrl = imageUrl;
		}

		public virtual string id { get; set; }
		public virtual Texture2D image	{ get; set; }
		public virtual string userName { get; set; }
		public virtual string userImageUrl { get; set; }

		public virtual bool isFriend {	get	{	return true;	}	}
		public virtual UserState state	{	get	{	return UserState.Online;	}	}
	}
}
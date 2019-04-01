// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace MPG.SocialPlatforms
{
	public class ErrorEventArgs : EventArgs
	{
		public string error;
		public int code;
		public bool isSucceeded;

		public ErrorEventArgs(string error, int code = 0)
		{
			this.error = error;
			this.code = code;
			isSucceeded = false;
		}

		public ErrorEventArgs()
		{
			isSucceeded = true;
		}
	}

	public class InitResultEventArgs : ErrorEventArgs
	{
		public ISocialNetwork socNet;
		public InitResultEventArgs() : base()
		{ }

		public InitResultEventArgs(ISocialNetwork socNet, string error) : base(error)
		{
			this.socNet = socNet;
		}
	}

	public class LoginResultEventArgs : ErrorEventArgs
	{
		public LoginResultEventArgs() : base() { }
		public LoginResultEventArgs(string error) : base(error) { }
	}

	public class MessagePostedEventArgs : EventArgs
	{
		public bool isPosted;

		public MessagePostedEventArgs(bool pPosted)
		{
			isPosted = pPosted;
		}
	}

	public class GetAppUsersEventArgs : ErrorEventArgs
	{
		public IUserProfile[] friends;

		public GetAppUsersEventArgs(IUserProfile[] friends) : base()
		{
			this.friends = friends;
		}

		public GetAppUsersEventArgs(string error) : base(error)
		{
		}
	}

	public class GetProfileEventArgs : ErrorEventArgs
	{
		public string name;
		public string lastName;
		public string photUrl;
		public Texture2D photo;

		public GetProfileEventArgs(string name, string lastName, string photUrl) : base()
		{
			this.name = name;
			this.lastName = lastName;
			this.photUrl = photUrl;
		}

		public GetProfileEventArgs(string name, Texture2D photo) : base()
		{
			this.name = name;
			this.photo = photo;
		}

		public GetProfileEventArgs(string error) : base(error)
		{ }

	}
}

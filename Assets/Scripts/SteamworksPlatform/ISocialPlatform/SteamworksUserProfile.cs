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
	using Steamworks;

	internal class SteamworksUserProfile : SocialUserProfileBase
	{
		protected Texture2D userImage;
		protected CSteamID steamID;
		protected Action imageLoadedCallback;

		private Callback<AvatarImageLoaded_t> avatarImageLoaded;

		public SteamworksUserProfile(string userName, string userId, string imageUrl) : base(userName, userId, imageUrl) {}

		public SteamworksUserProfile(CSteamID userId, string userName) : base(userName, userId.ToString(), string.Empty)
		{
			this.steamID = userId;
			base.userName = userName;

			avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(onDownloadImageComplete);
		}

		/// <summary>
		/// The user avatar downloading procedure
		/// </summary>
		/// <param name="callback">called when the avatar </param>
		public void DownloadImage(Action callback)
		{
			imageLoadedCallback = callback;
			int avatarHandle = SteamFriends.GetLargeFriendAvatar(steamID);

			switch (avatarHandle)
			{
				case 0:
					SteamworksPlatformConfig.Log.message("SteamworksUserProfile: " + userName + " no image");
					break; //no image
				case -1:
					//isLoading = true;
					SteamworksPlatformConfig.Log.message("SteamworksUserProfile: " + userName + " image is dowloading");
					break; //image is downloading
				default:
					loadImage(avatarHandle);
					SteamworksPlatformConfig.Log.message("SteamworksUserProfile: " + userName + " image is ready");
					break; //result is a handle id
			}

		}

		public void DownloadImage(Action<Texture2D> callback)
		{
			DownloadImage(() => callback(userImage));
		}

		void loadImage(int imageId)
		{
			uint ImageWidth;
			uint ImageHeight;
			bool ret = SteamUtils.GetImageSize(SteamFriends.GetLargeFriendAvatar(steamID), out ImageWidth, out ImageHeight);

			if (ret && ImageWidth > 0 && ImageHeight > 0)
			{
				//Log.warning("Friend image width: " + ImageWidth + " height: " + ImageHeight);

				byte[] Image = new byte[ImageWidth * ImageHeight * 4];
				ret = SteamUtils.GetImageRGBA(imageId, Image, (int)(ImageWidth * ImageHeight * 4));
				if (ret)
				{
					userImage = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);

					userImage.LoadRawTextureData(Image);
					userImage.Apply();
				}
				else
				{
					SteamworksPlatformConfig.Log.error("SteamworksUserProfile: image can't be loaded");
				}
			}
			else
			{
				SteamworksPlatformConfig.Log.error("SteamworksUserProfile: image can't be downloaded");
			}

			//isLoading = false;
			if(imageLoadedCallback != null)
				imageLoadedCallback();
		}

		void onDownloadImageComplete(AvatarImageLoaded_t param)
		{
			if (param.m_steamID == steamID)
			{
				avatarImageLoaded.Unregister();
				loadImage(param.m_iImage);
			}
		}

		public new string id
		{
			get
			{
				if(steamID.m_SteamID != 0)
					return steamID.m_SteamID.ToString();

				return base.id;
			}
		}

		public CSteamID SteamID
		{
			get { return steamID; }
		}
	}
}
#endif //!DISABLESTEAMWORKS
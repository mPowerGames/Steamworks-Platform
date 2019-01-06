// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

namespace MPG.SocialPlatforms
{
	internal static class SocialUtils
	{
		static ILogger logger;

		/// <summary>
		/// Logger instance
		/// </summary>
		internal static ILogger Log
		{
			get { return logger; }
			set { logger = value; }
		}
	}
}

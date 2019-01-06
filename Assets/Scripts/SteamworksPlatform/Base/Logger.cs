// This file is provided under The MIT License as part of ISocialPlatform implementation
// Copyright (c) 2018-2019 Alex Dovgodko
// Please see the included LICENSE.txt for additional information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MPG.SocialPlatforms
{
	/// <summary>
	/// Simple logger, intended to be overriden with your own logger if you have one
	/// </summary>
	internal class LoggerImpl : ILogger
	{
		public void message(string format, params object[] p) {
#if LOG_ON
			Debug.LogFormat(format, p);
#endif
		}
		public void warning(string format, params object[] p) { Debug.LogWarningFormat(format, p); }
		public void error(string format, params object[] p) { Debug.LogErrorFormat(format, p); }
		public void exception(System.Exception e) { Debug.LogException(e); }
	}
}

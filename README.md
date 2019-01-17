# Steamworks-Platform
Unity interface ISocialPlatform implementation using Steamworks.NET.
* [Unity®](https://unity3d.com) is a trademark of Unity Technologies.
* Steamworks is a trademark of Valve®.
* [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET) is a C# Wrapper for Valve's Steamworks API
* Steamworks-Platform Copyright (c) 2018-2019 Alex Dovgodko
* Feedback thread on [Unity forum](https://forum.unity.com/threads/isocialplatform-implementation-for-valves-steamworks-api.610084/)
## Overview
Steamworks Platform implements some features of the ISocialPlatform interface and only a few features of Steamworks API. The supported at the moment features are:
* Steam authorization and 
* Achievements loading and reporting
* Loading a friend list and acessing their profiles data (name and picture)

More features may be supported lately.
## Installation
1. First of all you have to fulfill all the requirements of Steamworks.NET and [install it](http://steamworks.github.io/installation/).
2. Download the package from [build\SteamworksSocial.unitypackage](https://github.com/mPowerGames/Steamworks-Platform/blob/master/Build/SteamworksSocial.unitypackage).
3. Launch Unity and open the project where you have already installed Steamworks.NET. 
4. In Unity import Steamworks Platform unitypackage by using main menu: __Assets -> Import Package -> Custom Package__ and confirm the import dialog.
5. In Unity open the player settings using main menu: __File -> Build Settings -> Player Settings__
You should now be ready to go.
## Demo Scene
The the package contains the demo scene that you may use to see some basic usage of the available features.
To run it you need to have the Steam client running. The demo scene is using a demo game from Steam (called Spacewar), so you may play with the available features and see how they are working. The only exception is a friends loading system as it is not directly supported by Steam and is implemented using some sort of a trick with leaderbord. So to test this feature, you will have to successfully launch at least one time the demo scene on the other Steam account, which is in your friend list.
## Limitations
As the functionality of the ISocialPlatform and Steamworks API are not idealy fit each other, some of the implemented features are supported with certain limitations.
* To use achievements, you have to use Social.Active.LoadAchievements (creating achievements localy is not supported).
* Achievements can not be completed directly (using IAchievement.completed), you will have to set it's progress to 100% with IAchievement.percentCompleted.
* Leaderboards are not implemented yet.
* ISocialPlatform.LoadFriends will download a list of friends for the localUser that have already installed and played your game.
* To acquire a friend picture you have to download it with an async call to MPG.SocialPlatforms.SteamworksUserProfile.DownloadImage.

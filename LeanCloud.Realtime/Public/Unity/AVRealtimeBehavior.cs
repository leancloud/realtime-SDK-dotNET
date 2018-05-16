using System;
using System.Collections;
using System.Collections.Generic;
using LeanCloud.Realtime.Internal;
using LeanCloud.Storage.Internal;
using LeanCloud.Core.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace LeanCloud.Realtime
{
    /// <summary>
    /// AVRealtime initialize behavior.
    /// </summary>
    public class AVRealtimeBehavior : AVInitializeBehaviour
	{
		void OnApplicationQuit()
        {
            List<AVRealtime> avRealtimeList = AVRealtime.avRealtimeList;
            for (int i = 0; i < avRealtimeList.Count; i++) {
                AVRealtime realtime = avRealtimeList[i];
                realtime.LogOut();
            }
        }
    }
}

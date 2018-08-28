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
            foreach (var item in AVRealtime.clients)
            {
                item.Value.LinkedRealtime.LogOut();
            }
        }

        private void Update()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                AVRealtime.PrintLog("unity Application.internetReachability is NetworkReachability.NotReachable");
                foreach (var item in AVRealtime.clients)
                {
                    item.Value.LinkedRealtime.StartAutoReconnect();
                }
            }
        }

    }
}

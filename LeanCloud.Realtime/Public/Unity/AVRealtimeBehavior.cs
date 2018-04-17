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
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            GameObject avRealtime = new GameObject("_AVRealtimeBehavior");
            DontDestroyOnLoad(avRealtime);
            avRealtime.AddComponent<AVRealtimeBehavior>();
            avRealtime.hideFlags = HideFlags.HideInHierarchy;
        }

        void OnDestroy()
        {

        }
    }
}

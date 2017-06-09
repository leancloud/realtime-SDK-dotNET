﻿using System;
using System.Collections;
using System.Collections.Generic;
using LeanCloud.Realtime.Internal;
using LeanCloud.Storage.Internal;
using LeanCloud.Core.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace LeanCloud.Realtime.Public.Unity
{
    /// <summary>
    /// AVRealtime initialize behavior.
    /// </summary>
    public class AVRealtimeBehavior : MonoBehaviour
    {
        private static bool isInitialized = false;
        private IDictionary<string, object> routerState;

        /// <summary>
        /// The LeanCloud applicationId used in this app. You can get this value from the LeanCloud website.
        /// </summary>
        [SerializeField]
        public string applicationID;

        /// <summary>
        /// The LeanCloud applicationKey used in this app. You can get this value from the LeanCloud website.
        /// </summary>
        [SerializeField]
        public string applicationKey;

        /// <summary>
        /// Whether use secure connection to get push router.
        /// </summary>
        [SerializeField]
        public bool secure;

        /// <summary>
        /// Gets or sets a value indicating whether this is web player.
        /// </summary>
        /// <value><c>true</c> if is web player; otherwise, <c>false</c>.</value>
        public static bool IsWebPlayer { get; set; }

        /// <summary>
        /// Initializes the LeanCloud SDK and begins running network requests created by LeanCloud.
        /// </summary>
        public virtual void Awake()
        {
            IsWebPlayer = Application.isWebPlayer;

            StartCoroutine(Initialize());

            // Force the name to be `AVRealtimeInitializeBehavior` in runtime.
            gameObject.name = "AVRealtimeInitializeBehavior";
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        public IEnumerator Initialize()
        {
            if (isInitialized)
            {
                yield break;
            }
            isInitialized = true;
            yield return FetchRouter();

        }

        IEnumerator FetchRouter()
        {
            var state = AVPlugins.Instance.AppRouterController.Get();
            var url = string.Format("https://{0}/v1/route?appId={1}", state.RealtimeRouterServer, applicationID);
            if (secure)
            {
                url += "&secure=1";
            }

            var request = new UnityWebRequest(url);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.Send();

            if (request.isError)
            {
                throw new AVException(AVException.ErrorCode.ConnectionFailed, "can not reach router.", null);
            }

            var result = request.downloadHandler.text;
            routerState = Json.Parse(result) as IDictionary<string, object>;
            if (routerState.Keys.Count == 0)
            {
                throw new KeyNotFoundException("Can not get websocket url from server,please check the appId.");
            }
            var ttl = long.Parse(routerState["ttl"].ToString());
            var expire = DateTime.Now.AddSeconds(ttl);
            routerState["expire"] = expire.UnixTimeStampSeconds();
        }
    }
}

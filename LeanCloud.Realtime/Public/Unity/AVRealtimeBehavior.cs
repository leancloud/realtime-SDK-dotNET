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
    public class AVRealtimeBehavior : MonoBehaviour
    {
        private static bool isInitialized = false;

        /// <summary>
        /// The LeanCloud applicationId used in this app. You can get this value from the LeanCloud website.
        /// </summary>
        [SerializeField]
        public string applicationID;

        /// <summary>
        /// Whether use secure connection to get push router.
        /// </summary>
        [SerializeField]
        public bool useSecure = true;

        /// <summary>
        /// Gets websocket server.
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; internal set; }


        /// <summary>
        /// Initializes the LeanCloud SDK and begins running network requests created by LeanCloud.
        /// </summary>
        public virtual void Awake()
        {
            // Force the name to be `AVRealtimeInitializeBehavior` in runtime.
            gameObject.name = "AVRealtimeInitializeBehavior";
        }

        public IEnumerator FetchRouter()
        {
            var prefix = applicationID.Substring(0, 8).ToLower();
            var router = string.Format("{0}.rtm.lncld.net", prefix);
            var url = string.Format("https://{0}/v1/route?appId={1}", router, applicationID);
            if (useSecure)
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
            var routerState = Json.Parse(result) as IDictionary<string, object>;
            if (routerState.Keys.Count == 0)
            {
                throw new KeyNotFoundException("Can not get websocket url from server,please check the appId.");
            }
            var ttl = long.Parse(routerState["ttl"].ToString());
            var expire = DateTime.Now.AddSeconds(ttl);
            routerState["expire"] = expire.UnixTimeStampSeconds();
            Server = routerState["server"].ToString();
        }
    }
}

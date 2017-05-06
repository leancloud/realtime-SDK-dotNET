using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.Internal
{
    internal class AVIMCorePlugins
    {
        private static readonly AVIMCorePlugins instance = new AVIMCorePlugins();
        public static AVIMCorePlugins Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly object mutex = new object();

        private IAVRouterController routerController;
        public IAVRouterController RouterController
        {
            get
            {
                lock (mutex)
                {
                    routerController = routerController ?? new AVRouterController();
                    return routerController;
                }
            }
            internal set
            {
                lock (mutex)
                {
                    routerController = value;
                }
            }
        }

        private IWebSocketClient webSocketController;

        public IWebSocketClient WebSocketController
        {
            get
            {
                lock (mutex)
                {
                    webSocketController = webSocketController ?? new DefaultWebSocketClient();
                    return webSocketController;
//#if MONO || UNITY
//                    if (webSocketController == null)
//                    {
//                        throw new NullReferenceException("must set a WebSocket client when call AVRealtime.Initialize(config)");
//                    }
//                    return webSocketController;
//#else
//                    webSocketController = webSocketController ?? new DefaultWebSocketClient();
//                    return webSocketController;
//#endif

                }
            }
            internal set
            {
                lock (mutex)
                {
                    webSocketController = value;
                }
            }
        }
        private IAVIMCommandRunner imCommandRunner;

        public IAVIMCommandRunner IMCommandRunner
        {
            get
            {
                lock (mutex)
                {
                    imCommandRunner = imCommandRunner ?? new AVIMCommandRunner(this.WebSocketController);
                    return imCommandRunner;
                }
            }
        }

        private IFreeStyleMessageClassingController freeStyleClassingController;
        public IFreeStyleMessageClassingController FreeStyleClassingController
        {
            get
            {
                lock (mutex)
                {
                    freeStyleClassingController = freeStyleClassingController ?? new FreeStyleMessageClassingController();
                    return freeStyleClassingController;
                }
            }
        }
    }
}

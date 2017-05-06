using LeanCloud.Core.Internal;
using LeanCloud.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.Internal
{
    internal class AVRouterController : IAVRouterController
    {
        const string routerUrl = "http://router.g0.push.leancloud.cn/v1/route?appId={0}&secure=1";
        const string routerKey = "LeanCloud_RouterState";
        public Task<PushRouterState> GetAsync(CancellationToken cancellationToken)
        {
            //return Task.FromResult(new PushRouterState()
            //{
            //    server = "wss://rtm57.leancloud.cn/"
            //});
            return LoadAysnc(cancellationToken).OnSuccess(_ =>
             {
                 var cache = _.Result;
                 var task = Task.FromResult<PushRouterState>(cache);

                 if (cache == null || cache.expire < DateTime.Now.UnixTimeStampSeconds())
                 {
                     task = QueryAsync(cancellationToken);
                 }

                 return task;
             }).Unwrap();
        }

        Task<PushRouterState> LoadAysnc(CancellationToken cancellationToken)
        {
            try
            {
                return AVPlugins.Instance.StorageController.LoadAsync().OnSuccess(_ =>
                 {
                     var currentCache = _.Result;
                     object routeCacheStr = null;
                     if (currentCache.TryGetValue(routerKey, out routeCacheStr))
                     {
                         var routeCache = routeCacheStr as IDictionary<string, object>;
                         var routerState = new PushRouterState()
                         {
                             groupId = routeCache["groupId"] as string,
                             server = routeCache["server"] as string,
                             secondary = routeCache["secondary"] as string,
                             ttl = long.Parse((routeCache["ttl"].ToString())),
                             source = "localCache"
                         };
                         return routerState;
                     }
                     return null;
                 });
            }
            catch
            {
                return Task.FromResult<PushRouterState>(null);
            }
        }
        Task<PushRouterState> QueryAsync(CancellationToken cancellationToken)
        {
            var appRouter = AVPlugins.Instance.AppRouterController.Get();
            var routerHost = string.Format("https://{0}/v1/route?appId={1}&secure=1", appRouter.RealtimeRouterServer,AVClient.CurrentConfiguration.ApplicationId) ?? appRouter.RealtimeRouterServer ?? string.Format(routerUrl, AVClient.CurrentConfiguration.ApplicationId);

            return AVClient.RequestAsync(uri: new Uri(routerHost),
                method: "GET",
                headers: null,
                data: null,
                contentType: "",
                cancellationToken: CancellationToken.None).ContinueWith<PushRouterState>(t =>
                {
                    var httpStatus = (int)t.Result.Item1;
                    if (httpStatus != 200)
                    {
                        throw new AVException(AVException.ErrorCode.ConnectionFailed, "can not reach router.", null);
                    }
                    try
                    {
                        var result = t.Result.Item2;

                        var routerState = Json.Parse(result) as IDictionary<string, object>;
                        if (routerState.Keys.Count == 0)
                        {
                            throw new KeyNotFoundException("Can not get websocket url from server,please check the appId.");
                        }
                        var ttl = long.Parse(routerState["ttl"].ToString());
                        var expire = DateTime.Now.AddSeconds(ttl);
                        routerState["expire"] = expire.UnixTimeStampSeconds();

                        //save to local cache async.
                        AVPlugins.Instance.StorageController.LoadAsync().OnSuccess(storage => storage.Result.AddAsync(routerKey, routerState));
                        var routerStateObj = new PushRouterState()
                        {
                            groupId = routerState["groupId"] as string,
                            server = routerState["server"] as string,
                            secondary = routerState["secondary"] as string,
                            ttl = long.Parse(routerState["ttl"].ToString()),
                            expire = expire.UnixTimeStampSeconds(),
                            source = "online"
                        };

                        return routerStateObj;
                    }
                    catch (Exception e)
                    {
                        if (e is KeyNotFoundException)
                        {
                            throw e;
                        }
                        return null;
                    }

                });
        }
    }
}

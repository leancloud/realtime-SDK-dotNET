## 安装
请下载 [leancloud/realtime-SDK-dotNET#releases](https://github.com/leancloud/realtime-SDK-dotNET/releases) 最新版的，然后将文件夹内的所有 dll 文件都依次引入，dll 清单如下：

- `AssemblyLister.dll`
- `LeanCloud.Core.dll`
- `LeanCloud.Realtime.dll`
- `LeanCloud.Storage.dll`
- `Unity.Compat.dll`
- `Unity.Tasks.dll`
- `Unityeditor.ios.extensions.xcode.dll` 注 这个比较特殊，请参看[Unity的已知问题](https://github.com/leancloud/unity-sdk#已知问题)
- `websocket-sharp.dll`

## 初始化 SDK

### 1.实现 WebSocketClient 接口

```cs
    /// <summary>
    /// LeanCloud WebSocket 客户端接口
    /// </summary>
    public interface IWebSocketClient
    {

        /// <summary>
        /// 客户端 WebSocket 长连接是否打开
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// WebSocket 长连接关闭时触发的事件回调
        /// </summary>
        event Action OnClosed;
        /// <summary>
        /// WebSocket 客户端遇到了错误时触发的事件回调
        /// </summary>
        event Action<string> OnError;

        /// <summary>
        /// 暂时留作日后打开日志跟踪时，当前版本并未调用，无需实现
        /// </summary>
        event Action<string> OnLog;
        /// <summary>
        /// 云端发送数据包给客户端，WebSocket 接受到时触发的事件回调
        /// </summary>
        event Action<string> OnMessage;

        /// <summary>
        /// 客户端 WebSocket 长连接成功打开时，触发的事件回调
        /// </summary>
        event Action OnOpened;

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        void Close();

        /// <summary>
        /// 打开连接
        /// </summary>
        /// <param name="url">wss 地址</param>
        /// <param name="protocol">子协议</param>
        void Open(string url, string protocol = null);
        /// <summary>
        /// 发送数据包的接口
        /// </summary>
        /// <param name="message"></param>
        void Send(string message);
    }
```
为了方便用户我们已经选用了一个在 Unity 上比较流行的一个库：[websocket-sharp](https://github.com/sta/websocket-sharp),并且基于这个库我们给出实现 `IWebSocketClient` 接口的代码如下，请在您的项目里面创建一个 .cs 文件，命名为「MyWebSokcetClient」 ：

```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeanCloud.Realtime.Internal;
using System;
using WebSocketSharp;

public class MyWebSockeyClient : IWebSocketClient
{
    WebSocket ws;
    public bool IsOpen
    {
        get
        {
            return ws.IsAlive;
        }
    }

    public event Action OnClosed;
    public event Action<string> OnError;
    public event Action<string> OnLog;
    public event Action<string> OnMessage;
    public event Action OnOpened;

    public void Close()
    {
        ws.CloseAsync();
    }

    public void Open(string url, string protocol = null)
    {
        Debug.Log("url:"+ url);
        ws = new WebSocket(url);
        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += Ws_OnMessage;
        ws.OnClose += Ws_OnClose;
        ws.ConnectAsync();
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        this.OnClosed();
    }

    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        this.OnMessage(e.Data);
    }

    private void Ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("Ws_OnOpen");
        this.OnOpened();
    }

    public void Send(string message)
    {
        ws.SendAsync(message, (b) =>
        {

        });
    }
}
```

### 2.将 「MyWebSocketClient」 作为参数传递给聊天模块初始化的函数
SDK 内置了一个 `AVInitializeBehaviour`,可以将其拖拽到任意一个 GameObject 上之后，然后输入LeanCloud 一个应用 appId 以及 appKey，然后在这个 GameObject 的 Start 方法里面输入如下代码：

```cs
public class LeanMessageTest : MonoBehaviour
{
    AVRealtime realtime;

    void Start()
    {
        var config = new AVRealtime.Configuration()
        {
            ApplicationId = "您的 appId",
            ApplicationKey = "您的 appKey",
            WebSocketClient = new MyWebSocketClient()// 这里可以换成您自己的实现
        };
        realtime = new AVRealtime(config);
    }
}
```

以上两步就是初始化所需要的步骤。下面是关于初始化的几个疑问解答。

### 为什么要输入两次 appId？
因为我们的聊天和存储逻辑上的分开的，只是因为聊天需要引用到存储 SDK 里面的 HTTP 请求的模块，因此聊天的需要依赖存储，但是并不会过多依赖，这也是为了之后聊天可以单独开源 SDK 做好准备。

### 为什么要开发者自己实现 IWebSocketClient 接口？ 
因为根据经验，游戏开发者更在乎的是实时性和互动性，聊天在游戏中的定位更为特殊，我们研究了很多在 Unity 上流行的 WebSocket 的第三方库，多多少少都有一些问题，很多在 .NET 原生上面比较好用的库在 Mono 上都缺乏对应的版本，因此我们选用了开源的 [websocket-sharp](https://github.com/sta/websocket-sharp)，如果开发者对第三方库要求更高的话建议可以购买来试一试：
[WebSocket for desktop, web and mobile](https://www.assetstore.unity3d.com/en/#!/content/27658)，开发者如果能够掌控客户端的 IWebSocketClient 就可以拥有更多的自主权，开发体验较好。

## 使用

### 1.建立与 LeanCloud 云端的长链接，构建 AVIMClient 对象

> 首先，需要开发者明确 `Client Id` 的概念，请一定确保阅读了 [用户和登录](https://leancloud.cn/docs/realtime_v2.html)

每一个游戏玩家都会在游戏里面拥有一个唯一独立的 ID，这个 ID 一般来说类似网易的梦幻西游是一个纯数字，但是在 LeanCloud 的聊天系统中，建议是一个少于 64 的任意的字符串，这样就可以方便游戏系统中可以直接将当前玩家的数字 ID 转化成字符串，直接作为 LeanCluod 聊天系统中的 Client ID 即可。

因此我们建议每一个玩家对应的就是 SDK 里面内置的一个对象：`AVIMClient`。

假设一个玩家的数字 ID 是 `1888888`，那么如下代码将演示他登录到 LeanCloud 聊天系统中：

```cs
public class LeanMessageTest : MonoBehaviour
{
    AVRealtime realtime;
    AVIMClient currentPlayer;
    void Start()
    {
        var config = new AVRealtime.Configuration()
        {
            ApplicationId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap",
            ApplicationKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww",
            WebSocketClient = new MyWebSocketClient()
        };
        realtime = new AVRealtime(config);
        // 传入玩家的 ID 作为 Client ID，构建一个 AVIMClient
        realtime.CreateClient("1888888").ContinueWith(t => 
        {
            // 此时已经登录成功
            currentPlayer = t.Result;
            
        });
    }
}
```
### 2.与其他玩家聊天
假设有一个玩家 「法师A」  的数字 ID 是 `1888888`，那么如下代码将演示在 LeanCloud 聊天系统中这个玩家在聊天系统中如何与另一个 ID 为 `2999999` 玩家 「战士B」 聊天的过程。

首先，在另外一台开发机器上，启动 Unity Editor，然后照着之前的代码，使用同一个 appId 和 appKey 初始化 SDK 之后，让 玩家「战士B」 以他的数字 ID `2999999` 作为 Client ID 登录。

首先，保证聊天的双方都与 LeanCloud 聊天的服务端建立的长链接，并且已经分别已自己的数字 ID 登录进了系统，然后 「法师A」开始发起一次聊天，这就要构建一个[对话](https://leancloud.cn/docs/realtime_v2.html#对话_Conversation_) 来实现这个功能：

#### 法师创建对话
在「法师A」 的界面上放置一个按钮(Button)，然后在按钮的点击事件里面编写如下代码： 

```cs
```

（先不用点击，还需要后续的步骤配合才能看见效果，别着急）

#### 战士准备好接受消息
回到 「战士B」 这边的 Unity Editor ，然后在「战士B」登录的界面上，防止一个 InputBox ，然后在登录的代码上加上如下代码：

```cs
``` 
以上代码是告知 SDK，「战士B」 开始接受消息了，如果有消息收到，请通知监听者(Listener)。


#### 法师发送消息

回到「法师A」 这里，在创建对话之后的 Continue 里面继续发送消息（每一个 Task 都是与服务端异步的交互，因此可以使用 Task.Continue 来进行这种链式语法的编程），代码如下：

```cs
``` 






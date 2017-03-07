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

### WebSocket 库的选择

为了方便用户我们已经内置了一个在 Unity 上比较流行的一个库：[websocket-sharp](https://github.com/sta/websocket-sharp),并且基于这个库实现了 SDK 内置的 `IWebSocketClient` 接口。

### 绑定 AVInitializeBehaviour 到 Main Camerra
SDK 内置了一个 `AVInitializeBehaviour`,可以将其拖拽到 Main Camerra 上之后，然后输入 LeanCloud 一个应用 appId 以及 appKey，新建一个 cs 文件，叫做 `ChatTest.cs`
然后在 ChatTest 的 Start 方法里面输入如下代码：

```cs
public class ChatTest : MonoBehaviour
{
    AVRealtime realtime;

    void Start()
    {
        var config = new AVRealtime.Configuration()
        {
            ApplicationId = "您的 appId",
            ApplicationKey = "您的 appKey",
        };
        realtime = new AVRealtime(config);
    }
}
```

以上两步就是初始化所需要的步骤。下面是关于初始化的几个疑问解答。

### 为什么要输入两次 appId？
因为我们的聊天和存储逻辑上的分开的，只是因为聊天需要引用到存储 SDK 里面的 HTTP 请求的模块，因此聊天的需要依赖存储，但是并不会过多依赖，这也是为了之后聊天可以单独开源 SDK 做好准备。

### 什么情况下开发者可以自定义实现 IWebSocketClient 接口？ 
因为根据经验，游戏开发者更在乎的是实时性和互动性，聊天在游戏中的定位更为特殊，我们研究了很多在 Unity 上流行的 WebSocket 的第三方库，多多少少都有一些问题，很多在 .NET 原生上面比较好用的库在 Mono 上都缺乏对应的版本，因此我们选用了开源的 [websocket-sharp](https://github.com/sta/websocket-sharp)，如果开发者对第三方库要求更高的话建议可以购买来试一试：
[WebSocket for desktop, web and mobile](https://www.assetstore.unity3d.com/en/#!/content/27658)，开发者如果能够掌控客户端的 IWebSocketClient 就可以拥有更多的自主权，开发体验较好，如果开发者自定义实现了 `IWebSocketClient` 接口，可以在初始化的时候指定：

```cs
public class ChatTest : MonoBehaviour
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

## 使用

### 1.建立与 LeanCloud 云端的长链接，构建 AVIMClient 对象

> 首先，需要开发者明确 `Client Id` 的概念，请一定确保阅读了 [用户和登录](https://leancloud.cn/docs/realtime_v2.html)

每一个游戏玩家都会在游戏里面拥有一个唯一独立的 ID，这个 ID 一般来说类似网易的梦幻西游是一个纯数字，但是在 LeanCloud 的聊天系统中，建议是一个少于 64 的任意的字符串，这样就可以方便游戏系统中可以直接将当前玩家的数字 ID 转化成字符串，直接作为 LeanCluod 聊天系统中的 Client ID 即可。

因此我们建议每一个玩家对应的就是 SDK 里面内置的一个对象：`AVIMClient`。

假设一个玩家的数字 ID 是 `1888888`，那么如下代码将演示他登录到 LeanCloud 聊天系统中：

```cs
public class ChatTest : MonoBehaviour
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
    AVIMConversation currentConveration;
    public void Create_Clicked()
    {
        // 输入对方的 ID
        var friendId = "2999999";
        // 以对方的 ID 和自己的 ID 作为成员创建一个对话，类似于
        currentPlayer.CreateConversationAsync(friendId).ContinueWith(t =>
        {
            currentConveration = t.Result;
        });
    }
```

#### 战士准备好接受消息
回到 「战士B」 这边的 Unity Editor ，然后在「战士B」登录的界面上，放置一个 Button，然后在事件点击中编写如下代码：

```cs
    // 战士自身登录
    avRealtime.CreateClient("2999999").ContinueWith(t =>
    {
        currentPlayer = t.Result;

        // 申明一个文本消息的监听者，它只监听协议里面的文本消息
        var textMessageListener = new AVIMTextMessageListener();
        // 设置接收到文本消息之后的事件回调
        textMessageListener.OnTextMessageReceieved += TextMessageListener_OnTextMessageReceieved;
        // 为当前用户绑定这个监听者
        currentPlayer.RegisterListener(textMessageListener);
    });
    private void TextMessageListener_OnTextMessageReceieved(object sender, AVIMTextMessageEventArgs e)
    {
        Debug.Log("received a text message:" + e.TextMessage.TextContent);
    }
``` 
以上代码是告知 SDK，「战士B」 开始接受消息了，如果有消息收到，请通知监听者(Listener)。


#### 法师发送消息

回到「法师A」 这里，放置一个发送按钮以及一个文本输入框用来输入消息，它的点击事件代码如下：

```cs
    GameObject messageInputField;
    void Start()
    {
        string appId = "uay57kigwe0b6f5n0e1d4z4xhydsml3dor24bzwvzr57wdap";
        string appKey = "kfgz7jjfsk55r5a8a3y4ttd3je1ko11bkibcikonk32oozww";
        avRealtime = new AVRealtime(appId, appKey);

        messageInputField = GameObject.FindGameObjectWithTag("message");
    }
    public void SendTextMessage()
    {
        if (currentConveration != null)
        {
            var text = messageInputField.GetComponent<InputField>().text.Trim();
            AVIMTextMessage textMessage = new AVIMTextMessage(text);
            currentConveration.SendMessageAsync(textMessage);
        }
    }
``` 

然后 「战士B」 就可以收到消息了。反过来，如果战士也想发消息给法师，就复制上述步骤即可。 






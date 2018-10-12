
# 更新日志


## 2018-10-12

### 新增消息记录翻页器 

```cs
var conversation = await tom.GetConversationAsync("5b83a01a5b90c830ff80aea4");

var messagePager = conversation.GetMessagePager().SetPageSize(2);

var pager1 = await messagePager.PreviousAsync();
var pager2 = await messagePager.PreviousAsync();
Console.WriteLine(pager1.Count());
```

### 新增消息查询类

```cs
var messageQuery = conversation.GetMessageQuery();
// 查询最新的 20 条图像消息
var latestImageMessageWithCount20 = await messageQuery.FindAsync<AVIMImageMessage>();
Console.WriteLine(latestImageMessageWithCount20.Count());
```

## 2018-10-11

### 新增 Builder 模式构建消息发送器

`AVIMMessageEmitterBuilder` 的用法如下：

```cs
AVIMClient tom = await realtime.CreateClientAsync(clientId);

var conversation = await tom.GetConversationAsync("5b83a01a5b90c830ff80aea4");

var message = new AVIMTextMessage()
{
    TextContent = "Jerry，今晚有比赛，我约了 Kate，咱们仨一起去酒吧看比赛啊？！"
};

AVIMSendOptions sendOptions = new AVIMSendOptions()
{
    Will = true
};

var emitterBuilder = new AVIMMessageEmitterBuilder()
    .SetConversation(conversation)
    .SetMessage(message)
    .SetSendOptions(sendOptions);
await emitterBuilder.SendAsync();
```


## 2018-09-25

### 支持使用 Builder 模式构建对话

```cs
var conversationBuilder = tom.GetConversationBuilder().SetProperty("type", "private").SetProperty("pinned", true);
var conversation = await tom.CreateConversationAsync(conversationBuilder);
```

### 重新设计了新版的内置消息类型

发送图像消息：

```cs
// 外链
await conversation.SendImageAsync("http://ww3.sinaimg.cn/bmiddle/596b0666gw1ed70eavm5tg20bq06m7wi.gif", "Satomi_Ishihara", "萌妹子一枚", new Dictionary<string, object>
{
   { "actress","石原里美"}
});
// 物理文件
using (FileStream fileStream = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "San_Francisco.jpg"), FileMode.Open, FileAccess.Read))
{
   var imageMessage = new AVIMImageMessage();
   imageMessage.File = new AVFile("San_Francisco.png", fileStream);
   imageMessage.TextContent = "发自我的 Windows";
   await conversation.SendAsync(imageMessage);
}
```

### 查询消息历史记录支持泛型查询子类

```cs
// 不指定类型，获取 10 条最近的消息
var messages = await conversation.QueryMessageAsync(limit: 10);
// 指定图像消息类型
var imageMessages = await conversation.QueryMessageAsync<AVIMImageMessage>(limit: 10);
```

## 2018-04-05

### 自动重连的策略更新

之前的逻辑如下：

1. 如果断线，会重连之前的 websocket 地址（prefered server）
2. 并且会一直在 session token 有效期内使用当前的服务器地址

现在的逻辑如下：

1. 如果断线，会重连之前的 websocket 地址（prefered server）
2. 如果重连三次 prefered address 失败，则会切换到 secondary server，并且会重新登录（签名和 session token 都会重新获取并且刷新）
3. 如果 secondary server 也重连失败 3 次，则会重新请求 rtm router 地址，重新获取 prefered server 和 secondary server
4. 重复第一步


## 2017-12-26

### 支持了消息的撤回和修改

消息撤回：

```cs
await this.client.RecallAsync(receivedMessage);// receivedMessage 只要实现了 IAVIMMessage 接口就可以
```

消息修改：
```cs
await this.client.ModifyAysnc(receivedMessage);// receivedMessage 只要实现了 IAVIMMessage 接口就可以
```

而在对话内的其他用户，都会触发对应的事件通知：

```cs
client.OnMessageRecalled += Client_OnMessageRecalled;
private void Client_OnMessageRecalled(object sender, AVIMMessagePatchEventArgs e)
{
    var list = e.Messages.ToList();
    Console.WriteLine(list[0].Id + " has been recalled.");
}          
```

```cs
client.OnMessageModified += Client_OnMessageModified;
private vood Client_OnMessageModified(object sender, AVIMMessagePatchEventArgs e))
{
    var list = e.Messages.ToList();
    Console.WriteLine(list[0].Id + " has been modified.");
}
```

## 2017-12-13
### 支持了已读回执和发送回执的分离

```cs
var realtimeConfig = new AVRealtime.Configuration()
{
    ApplicationId = appId,
    ApplicationKey = appkey,
    OfflineMessageStrategy = AVRealtime.OfflineMessageStrategy.UnreadAck,//开始支持主动发送已读回执
};

realtime = new AVRealtime(realtimeConfig);
```

#### 增加了 AVIMConversation.ReadAsync 和 AVIMClient.ReadAllAsync 接口

```cs
// 告知服务端，当前用户标识自己已经读取了该对话的最新消息，更新已读回执
conversation.ReadAsync();
```

```cs
// 告知服务端，当前用户标识自己所有的对话都已经读取到最新，更新所有的已读回执
client.ReadAllAsync();
```

#### 增加 AVIMConversation.Unread 可以获取当前对话的未读消息状态

```cs
var state = conversation.Unread;
Console.WriteLine(state.UnreadCount);//未读消息数量
Console.WriteLine(state.LastUnreadMessage.Id);//最新一条未读消息的 ID
Console.WriteLine(state.SyncdAt);//最后同步的时间戳
```
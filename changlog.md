
# 更新日志
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
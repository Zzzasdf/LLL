namespace ChatModule
{
    public partial class ChatManager: IChatManager
    {
        void IInit.OnInit()
        {
            // 初始化配置
            InitConfigure();
            // 初始化频道配置
            InitChannel();
        }
        void IReset.OnReset()
        {
        }

        void IDestroy.OnDestroy()
        {
        }
        
        /// 添加源数据
        public void AddSource(IChatData chatData)
        {
            ChatMessage chatMessage = ChatMessagePool.Get(chatData);
            // 聊天室
            foreach (var pair in rooms)
            {
                if (pair.Value.TryAddMessage(chatMessage, out bool isReleaseFirstMessage))
                {
                    // 成功添加到指定房间后的操作，可用来发送事件给对应的模块，
                    // 也可用 Room 内置事件
                }
            }
            // 聊天屋
            foreach (var pair in houses)
            {
                if (pair.Value.TryAddMessage(chatMessage, out bool isReleaseFirstRoom, out bool isReleaseRoomFirstMessage))
                {
                    // 成功添加到指定房间后的操作，可用来发送事件给对应的模块，
                    // 也可用 Room 内置事件
                }
            }
            ChatMessagePool.Release(chatMessage);
        }
    }
}
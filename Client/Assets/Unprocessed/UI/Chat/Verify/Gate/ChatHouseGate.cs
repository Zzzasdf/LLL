namespace ChatModule
{
    /// 聊天屋大门
    public interface IChatHouseGate
    {
        /// 是否允许添加该消息
        bool AllowAddNewMessage(ChatMessage chatMessage);
    }

    /// 频道聊天屋大门
    public class ChatHouseChannelGate : IChatHouseGate
    {
        private EChatChannel eChatChannel;
        
        public ChatHouseChannelGate(EChatChannel eChatChannel)
        {
            this.eChatChannel = eChatChannel;
        }
        bool IChatHouseGate.AllowAddNewMessage(ChatMessage chatMessage)
        {
            return chatMessage.EChatChannel() != eChatChannel;
        }
    }
}
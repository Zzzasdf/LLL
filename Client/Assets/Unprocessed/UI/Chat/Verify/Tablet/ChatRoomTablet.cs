using UnityEngine;

namespace ChatModule
{
    /// 聊天室门牌
    public interface IChatRoomTablet
    {
        /// 初始化门牌
        void Init(ChatMessage chatMessage);
        /// 深拷贝
        IChatRoomTablet Clone();
        /// 是否允许添加该消息
        bool AllowAddNewMessage(ChatMessage chatMessage);
    }

    /// 频道聊天室门牌
    public class ChatRoomChannelTablet: IChatRoomTablet
    {
        private EChatChannel eChatChannel;
        
        public ChatRoomChannelTablet(EChatChannel eChatChannel)
        {
            this.eChatChannel = eChatChannel;
        }
        void IChatRoomTablet.Init(ChatMessage chatMessage)
        {
            Debug.LogError("该类型门牌请在构造器中初始化！！");
        }
        IChatRoomTablet IChatRoomTablet.Clone()
        {
            return new ChatRoomChannelTablet(eChatChannel);
        }
        bool IChatRoomTablet.AllowAddNewMessage(ChatMessage chatMessage)
        {
            return chatMessage.EChatChannel() == eChatChannel;
        }
    }

    /// 私聊室门牌
    public class ChatRoomPrivateTablet : IChatRoomTablet
    {
        private EChatChannel eChatChannel;
        private long selfId;
        private long targetId;
        
        void IChatRoomTablet.Init(ChatMessage chatMessage)
        {
            eChatChannel = EChatChannel.Private;
            selfId = 0;
            targetId = 0;
            if (chatMessage.ChatData() is not IResChat resChat) return;
            selfId = resChat.SenderId();
            targetId = resChat.TargetId();
        }
        IChatRoomTablet IChatRoomTablet.Clone()
        {
            ChatRoomPrivateTablet result = new ChatRoomPrivateTablet();
            result.eChatChannel = eChatChannel;
            result.selfId = selfId;
            result.targetId = targetId;
            return result;
        }
        bool IChatRoomTablet.AllowAddNewMessage(ChatMessage chatMessage)
        {
            if (chatMessage.EChatChannel() != eChatChannel) return false;
            if (chatMessage.ChatData() is not IResChat resChat) return false;
            if (resChat.SenderId() == selfId && resChat.TargetId() == targetId) return true;
            if (resChat.SenderId() == targetId && resChat.TargetId() == selfId) return true;
            return false;
        }
    }

    /// 群组室门牌
    public class ChatRoomGroupTablet : IChatRoomTablet
    {
        private EChatChannel eChatChannel;
        private long groupId;
        
        void IChatRoomTablet.Init(ChatMessage chatMessage)
        {
            eChatChannel = EChatChannel.Group;
            groupId = 0;
            if (chatMessage.ChatData() is not IResChat resChat) return;
            groupId = resChat.GroupId();
        }
        IChatRoomTablet IChatRoomTablet.Clone()
        {
            ChatRoomGroupTablet result = new ChatRoomGroupTablet();
            result.eChatChannel = eChatChannel;
            result.groupId = groupId;
            return result;
        }
        bool IChatRoomTablet.AllowAddNewMessage(ChatMessage chatMessage)
        {
            if (chatMessage.EChatChannel() != eChatChannel) return false;
            if (chatMessage.ChatData() is not IResChat resChat) return false;
            return resChat.GroupId() == groupId;
        }
    }
}
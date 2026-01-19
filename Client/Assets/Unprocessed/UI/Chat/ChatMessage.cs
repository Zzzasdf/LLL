using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace ChatModule
{
    public enum EChatDataType
    {
        /// 无效
        Invalid = 0,
        /// 聊天
        ResChat = 1,
        /// 公告
        ResAnnounce = 2,
    }

    public interface IChatData
    {
        
    }
    public interface IResChat: IChatData
    {
        EChatChannel EChatChannel();
        long SenderId();
        long TargetId();
        long GroupId();
        string Message();
    }
    public interface IResAnnounce: IChatData
    {
        EChatChannel EChatChannel();
        string Message();
    }

    public interface IChatMessage
    {
        void AddSource(IChatData chatData);
        void Clear();
        ChatMessage Clone();
        void AddParser(IParser parser);
    }
    /// 聊天通用数据结构
    public class ChatMessage: IChatMessage
    {
        /// 消息解析器
        private IParser parser;
        /// 消息解析缓存
        private string message;
        /// 源数据
        private IChatData chatData;

        void IChatMessage.AddSource(IChatData chatData)
        {
            this.chatData = chatData;
        }
        void IChatMessage.Clear()
        {
            parser = null;
            message = null;
            chatData = null;
        }
        ChatMessage IChatMessage.Clone()
        {
            return ChatMessagePool.Get(chatData);
        }
        void IChatMessage.AddParser(IParser parser)
        {
            this.parser = parser;
        }

        public string GetMessage()
        {
            if (string.IsNullOrEmpty(message))
            {
                message = parser.Encode(this);
            }
            return message;
        }

        /// 频道类型
        public EChatChannel EChatChannel()
        {
            switch (chatData)
            {
                case IResChat resChat: return resChat.EChatChannel();
                case IResAnnounce resAnnounce: return resAnnounce.EChatChannel();
                default:
                {
                    Debug.LogError($"不支持的当前数据类型 => {chatData.GetType()}");
                    return ChatModule.EChatChannel.Invalid;
                }
            }
        }

        /// 数据
        public IChatData ChatData() => chatData;
        /// 数据类型
        public EChatDataType EChatDataType()
        {
            switch (chatData)
            {
                case IResChat: return ChatModule.EChatDataType.ResChat;
                case IResAnnounce: return ChatModule.EChatDataType.ResAnnounce;
                default:
                {
                    Debug.LogError($"不支持的当前数据类型 => {chatData.GetType()}");
                    return ChatModule.EChatDataType.Invalid;
                }
            }
        }
        /// 获取强转类型数据
        public bool TryGetValue<T>(out T value)
            where T : class, IChatData
        {
            if (chatData == null)
            {
                Debug.LogError("当前的数据类型为空！！");
                value = null;
                return false;
            }
            if (chatData is not T t)
            {
                Debug.LogError($"与实际类型不符，预获取类型 => {typeof(T)}, 目标实际类型 => {chatData.GetType()}");
                value = null;
                return false;
            }
            value = t;
            return true;
        }
    }

    /// 数据结构池
    public static class ChatMessagePool
    {
        private static readonly ObjectPool<ChatMessage> s_Pool = new ObjectPool<ChatMessage>
        (
            () => new ChatMessage(), 
            null, 
            x => ((IChatMessage)x).Clear()
        );
        public static ChatMessage Get(IChatData chatData)
        {
            ChatMessage result = s_Pool.Get();
            ((IChatMessage)result).AddSource(chatData);
            return result;
        }
        public static void Release(ChatMessage toRelease) => s_Pool.Release(toRelease);
    }

    /// 数据结构节点池
    public static class ChatMessageNodePool
    {
        private static readonly ObjectPool<LinkedListNode<ChatMessage>> s_Pool = new ObjectPool<LinkedListNode<ChatMessage>>
        (
            () => new LinkedListNode<ChatMessage>(null), 
            null, 
            x =>
            {
                ChatMessagePool.Release(x.Value);
                x.Value = null;
            }
        );
        public static LinkedListNode<ChatMessage> Get(ChatMessage chatMessage)
        {
            LinkedListNode<ChatMessage> result = s_Pool.Get();
            result.Value = chatMessage;
            return result;
        }
        public static void Release(LinkedListNode<ChatMessage> toRelease) => s_Pool.Release(toRelease);
    }
}
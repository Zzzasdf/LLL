using System;
using System.Collections.Generic;

namespace ChatModule
{
    public interface IChatRoom
    {
        void Release();
        /// 生成一个全新的同类型房间
        IChatRoom BrandNew(out IChatRoomTablet chatRoomTablet);
    }
    
    /// 聊天室
    public class ChatRoom: IChatRoom
    {
        /// 门牌，是否允许消息进入该房间
        private IChatRoomTablet tablet;
        /// 房间消息容量
        private int capacity;
        /// 消息解析器
        private IParser parser;
        /// 消息集合
        private LinkedList<ChatMessage> chatMessages;

        /// 可选零件
        private Dictionary<Type, IChatRoomUnit> chatRoomUnits;

        /// 复用 Message 后的回调
        private Action afterReleaseMessageCallback;
        /// 新增 Message 后的回调
        private Action<ChatMessage> afterAddNewMessageCallback;
        
        public ChatRoom(IChatRoomTablet tablet, int capacity, IParser parser)
        {
            this.tablet = tablet;
            this.capacity = capacity;
            this.parser = parser;
            chatMessages = new LinkedList<ChatMessage>();
        }
        void IChatRoom.Release()
        {
            // 只复用于同类型的房间，只回收消息，和解绑回调
            while (chatMessages.Count > 0)
            {
                ChatMessageNodePool.Release(chatMessages.First);
                chatMessages.RemoveFirst();
            }
            // 零件清零
            if (chatRoomUnits != null)
            {
                foreach (var pair in chatRoomUnits)
                {
                    pair.Value.Clear();
                }
            }
            afterReleaseMessageCallback = null;
            afterAddNewMessageCallback = null;
        }
        IChatRoom IChatRoom.BrandNew(out IChatRoomTablet chatRoomTablet)
        {
            ChatRoom result = new ChatRoom(tablet.Clone(), capacity, parser);
            chatRoomTablet = result.tablet;
            return result;
        }

        public ChatRoom AttachUnit<T>(T unit)
            where T: IChatRoomUnit
        {
            chatRoomUnits ??= new Dictionary<Type, IChatRoomUnit>();
            chatRoomUnits.Add(typeof(T), unit);
            return this;
        }
        public bool TryGetUnit<T>(out T unit)
            where T: class, IChatRoomUnit
        {
            unit = null;
            if (chatRoomUnits == null) return false;
            if (!chatRoomUnits.TryGetValue(typeof(T), out IChatRoomUnit chatRoomUnit))
            {
                return false;
            }
            unit = chatRoomUnit as T;
            return true;
        }

        /// 尝试添加消息，当不满足 Room 的添加条件时，不添加
        /// 是否回收了 第一条消息结构
        public bool TryAddMessage(ChatMessage chatMessage, out bool isReleaseFirstMessage)
        {
            isReleaseFirstMessage = false;
            if (tablet?.AllowAddNewMessage(chatMessage) == false)
            {
                // 该条消息不满足条件, 不允许进入
                return false;
            }
            AddMessage(chatMessage, out isReleaseFirstMessage);
            return true;
        }
        /// 添加进聊天记录，并检测容量，回收利用
        /// 是否回收了 第一条消息结构
        private void AddMessage(ChatMessage chatMessage, out bool isReleaseFirstMessage)
        {
            // 浅拷贝一份 只引用源数据
            ChatMessage clone = ((IChatMessage)chatMessage).Clone();
            // 将 解析器 添加 进 消息
            ((IChatMessage)clone).AddParser(parser);
            
            // 检测当前是否已达容量上限
            isReleaseFirstMessage = chatMessages.Count >= capacity;
            if (isReleaseFirstMessage)
            {
                // 回收优先回收第一个节点
                ChatMessageNodePool.Release(chatMessages.First);
                chatMessages.RemoveFirst(); // 移除第一个节点
                // 触发回收 Message 回调
                FireAfterReleaseMessage();
            }
            // 生成新的节点, 添加在末尾
            LinkedListNode<ChatMessage> node = ChatMessageNodePool.Get(clone);
            chatMessages.AddLast(node);
            // 触发新增 Message 回调
            FireAfterAddNewMessage(clone);
        }

        /// 获取该房间内的所有消息
        /// eg！！请勿在外部做增删改操作
        public LinkedList<ChatMessage> AllMessage()
        {
            return chatMessages;
        }

        /// 尝试获取最后一条 消息
        /// eg！！请勿在外部做增删改操作
        public bool TryGetLastMessage(out ChatMessage chatMessage)
        {
            chatMessage = null;
            if (chatMessages.Count == 0)
            {
                return false;
            }
            chatMessage = chatMessages.Last.Value;
            return true;
        }

#region 订阅
        /// 订阅第一个 Message 被回收后的回调（有订阅，必有取消）
        public void SubscribeAfterReleaseMessage(Action reuseAfterCallback)
        {
            afterReleaseMessageCallback += reuseAfterCallback;
        }
        /// 取消订阅第一个 Message 被回收后的回调
        public void UnsubscribeAfterReleaseMessage(Action reuseAfterCallback)
        {
            afterReleaseMessageCallback -= reuseAfterCallback;
        }
        /// 内置触发 第一个 Message 被回收后的回调
        private void FireAfterReleaseMessage()
        {
            afterReleaseMessageCallback?.Invoke();
        }
        
        /// 订阅新增 Message 后的回调（有订阅，必有取消）
        public void SubscribeAfterAddNewMessage(Action<ChatMessage> addNewAfterCallback)
        {
            afterAddNewMessageCallback += addNewAfterCallback;
        }
        /// 取消订阅新增 Message 后的回调
        public void UnsubscribeAfterAddNewMessage(Action<ChatMessage> addNewAfterCallback)
        {
            afterAddNewMessageCallback -= addNewAfterCallback;
        }
        /// 内置触发 新增 Message 后的回调
        private void FireAfterAddNewMessage(ChatMessage chatMessage)
        {
            afterAddNewMessageCallback?.Invoke(chatMessage);
        }
#endregion
    }
}

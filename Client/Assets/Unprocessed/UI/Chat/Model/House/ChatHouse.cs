using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace ChatModule
{
    public interface IChatHouse
    {
        bool TryAddMessage(ChatMessage chatMessage, out bool isReleaseFirstRoom, out bool isReleaseRoomFirstMessage);
    }
    
    /// 聊天屋，多种同类型聊天室
    public class ChatHouse: IChatHouse
    {
        /// 大门，是否允许消息进入
        private IChatHouseGate chatHouseGate;
        /// 房间数量上限
        private int roomCapacity;
        
        /// 房间模板
        private ChatRoom chatRoomTemplate;
        /// 聊天室节点池
        private ChatRoomNodePool chatRoomNodePool;
        /// 房间集合
        private LinkedList<ChatRoom> chatBaseRooms;
        
        /// 复用 Room 后的回调
        private Action afterReleaseRoomCallback;
        /// 新增 Room 后的回调
        private Action<ChatRoom> afterAddNewRoomCallback;
        
        public ChatHouse(IChatHouseGate chatHouseGate, int roomCapacity, ChatRoom chatRoomTemplate)
        {
            this.chatHouseGate = chatHouseGate;
            this.roomCapacity = roomCapacity;
            this.chatRoomTemplate = chatRoomTemplate;
            chatRoomNodePool = new ChatRoomNodePool();
            chatBaseRooms = new LinkedList<ChatRoom>();
        }
        
        /// 尝试添加消息，当不满足 Room 的添加条件时，不添加
        /// 是否回收了 第一个Room、对应 Room 的 第一条消息结构
        public bool TryAddMessage(ChatMessage chatMessage, out bool isReleaseFirstRoom, out bool isReleaseRoomFirstMessage)
        {
            isReleaseFirstRoom = false;
            isReleaseRoomFirstMessage = false;
            if (chatHouseGate?.AllowAddNewMessage(chatMessage) == false)
            {
                // 该条消息不满足条件, 不允许进入
                return false;
            }
            AddMessage(chatMessage, out isReleaseFirstRoom, out isReleaseRoomFirstMessage);
            return true;
        }
        /// 添加进聊天记录，并检测容量，回收利用
        /// 是否回收了 第一个Room、对应 Room 的 第一条消息结构
        private void AddMessage(ChatMessage chatMessage, out bool isReleaseFirstRoom, out bool isReleaseRoomFirstMessage)
        {
            isReleaseFirstRoom = false;
            isReleaseRoomFirstMessage = false;
            // 判断是否存在该房间
            foreach (var room in chatBaseRooms)
            {
                if (!room.TryAddMessage(chatMessage, out isReleaseRoomFirstMessage))
                {
                    continue;
                }
                return;
            }
            // 该房间未生成，生成一个
            {
                isReleaseFirstRoom = chatBaseRooms.Count >= roomCapacity;
                if (isReleaseFirstRoom)
                {
                    // 回收优先回收第一个节点
                    chatRoomNodePool.Release(chatBaseRooms.First);
                    chatBaseRooms.RemoveFirst(); // 移除第一个节点
                    // 触发回收 Room 回调
                    FireAfterReleaseRoom();
                }
                // 生成新的房间节点，添加在末尾
                LinkedListNode<ChatRoom> node = chatRoomNodePool.Get(chatRoomTemplate, chatMessage);
                node.Value.TryAddMessage(chatMessage, out isReleaseRoomFirstMessage);
                chatBaseRooms.AddLast(node);
                // 触发新增 Room 回调
                FireAfterAddNewRoom(node.Value);
            }
        }
        
#region 订阅
        /// 订阅第一个 Room 被回收后的回调（有订阅，必有取消）
        public void SubscribeAfterReleaseRoom(Action reuseAfterCallback)
        {
            afterReleaseRoomCallback += reuseAfterCallback;
        }
        /// 取消订阅第一个 Room 被回收后的回调
        public void UnsubscribeAfterReleaseRoom(Action reuseAfterCallback)
        {
            afterReleaseRoomCallback -= reuseAfterCallback;
        }
        /// 内置触发 第一个 Room 被回收后的回调
        private void FireAfterReleaseRoom()
        {
            afterReleaseRoomCallback?.Invoke();
        }
        
        /// 订阅新增 Room 后的回调（有订阅，必有取消）
        public void SubscribeAfterAddNewRoom(Action<ChatRoom> addNewAfterCallback)
        {
            afterAddNewRoomCallback += addNewAfterCallback;
        }
        /// 取消订阅新增 Room 后的回调
        public void UnsubscribeAfterAddNewRoom(Action<ChatRoom> addNewAfterCallback)
        {
            afterAddNewRoomCallback -= addNewAfterCallback;
        }
        /// 内置触发 新增 Room 后的回调
        private void FireAfterAddNewRoom(ChatRoom chatRoom)
        {
            afterAddNewRoomCallback?.Invoke(chatRoom);
        }
#endregion
        
        /// 内置聊天室节点池
        private class ChatRoomNodePool
        {
            private readonly ObjectPool<LinkedListNode<ChatRoom>> s_Pool = new ObjectPool<LinkedListNode<ChatRoom>>(
                () => new LinkedListNode<ChatRoom>(null), 
                null, 
                x => ((IChatRoom)x.Value).Release(),
                null,
                true,
                10,1000
            );
            public LinkedListNode<ChatRoom> Get(ChatRoom chatRoomTemplate, ChatMessage chatMessage)
            {
                bool isFromPool = s_Pool.CountInactive > 0;
                LinkedListNode<ChatRoom> result = s_Pool.Get();
                if (!isFromPool)
                {
                    IChatRoom chatRoom = ((IChatRoom)chatRoomTemplate).BrandNew(out IChatRoomTablet chatRoomTablet);
                    chatRoomTablet.Init(chatMessage);
                    result.Value = (ChatRoom)chatRoom;
                }
                return result;
            }
            public void Release(LinkedListNode<ChatRoom> toRelease) => s_Pool.Release(toRelease);
        }
    }
}
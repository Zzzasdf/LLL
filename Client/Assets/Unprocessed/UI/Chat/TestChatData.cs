using System.Collections.Generic;
using UnityEngine;

namespace ChatModule
{
    public class ResChat : IResChat
    {
        private string message;
        public ResChat(string message)
        {
            this.message = message;
        }
        
        EChatChannel IResChat.EChatChannel() => EChatChannel.World;
        long IResChat.SenderId() => 0;
        long IResChat.TargetId() => 0;
        long IResChat.GroupId() => 0;
        string IResChat.Message() => message;
    }
    
    public class TestChatData : MonoBehaviour
    {
        [SerializeField] private UIChatMessageLoopScrollRect uiChatMessageLoopScrollRect;
        
        private List<string> strs = new List<string>
        {
            "AAAAAAAAAAAAAAAAAAAAAA",
            "BBBBBBBBBBBBBBBBBBBBB",
            "CCCCCCCCCCCCCCCCCCCCCC",
            "DDDDDDDDDDDDD",
            "EEEEEEEEEEEEEEEEEEEEEEE",
            "FFFFFFFFFFFFFFFFFFFFFF",
            "GGGGGGGGGGGGGGGGGGGGG",
            "HHHHHHHHHHHHHHHHHHHHHHH",
            "IIIIIIIIIII",
            "JJJJJJJJJJJJJJJJJJJJJJJ",
            "KKKKKKKKKKKKKKKKKKKKKKK",
            "LLLLLLLLLLLLLL"
        };
        
        private void Awake()
        {
            ChatManager chatManager = new ChatManager();
            for (int i = 0; i < strs.Count; i++)
            {
                ResChat resChat = new ResChat(strs[i]);
                chatManager.AddSource(resChat);
            }
            if (chatManager.TryGetRoom(EChatRoom.World, out ChatRoom chatRoom))
            {
                uiChatMessageLoopScrollRect.Init(chatRoom);
            }
        }
    }
}
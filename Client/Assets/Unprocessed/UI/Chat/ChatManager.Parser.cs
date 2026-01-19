using UnityEngine;

namespace ChatModule
{
    /// 解析器
    public interface IParser
    {
        string Encode(ChatMessage chatMessage);
    }

    /// 频道解析器
    public class ChannelParser: IParser
    {
        public string Encode(ChatMessage chatMessage)
        {
            switch (chatMessage.ChatData())
            {
                case IResChat resChat:
                {
                    // TODO ZZZ
                    return resChat.Message();
                }
                case IResAnnounce resAnnounce:
                {
                    // TODO ZZZ
                    return resAnnounce.Message();
                }
                default:
                {
                    string errorLog = $"未支持的类型 => {chatMessage.ChatData().GetType()}";
                    Debug.LogError(errorLog);
                    return errorLog;
                }
            }
        }
    }

    /// 简易解析器
    public class SimpleParser : IParser
    {
        public string Encode(ChatMessage chatMessage)
        {
            switch (chatMessage.ChatData())
            {
                case IResChat resChat:
                {
                    // TODO ZZZ
                    return resChat.Message();
                }
                case IResAnnounce resAnnounce:
                {
                    // TODO ZZZ
                    return resAnnounce.Message();
                }
                default:
                {
                    string errorLog = $"未支持的类型 => {chatMessage.ChatData().GetType()}";
                    Debug.LogError(errorLog);
                    return errorLog;
                }
            }
        }
    }

    /// 弹幕解析器
    public class BarrageParser : IParser
    {
        public string Encode(ChatMessage chatMessage)
        {
            switch (chatMessage.ChatData())
            {
                case IResChat resChat:
                {
                    // TODO ZZZ
                    return resChat.Message();
                }
                case IResAnnounce resAnnounce:
                {
                    // TODO ZZZ
                    return resAnnounce.Message();
                }
                default:
                {
                    string errorLog = $"未支持的类型 => {chatMessage.ChatData().GetType()}";
                    Debug.LogError(errorLog);
                    return errorLog;
                }
            }
        }
    }

    /// 私聊解析器
    public class PrivateParser : IParser
    {
        public string Encode(ChatMessage chatMessage)
        {
            switch (chatMessage.ChatData())
            {
                case IResChat resChat:
                {
                    // TODO ZZZ
                    return resChat.Message();
                }
                default:
                {
                    string errorLog = $"未支持的类型 => {chatMessage.ChatData().GetType()}";
                    Debug.LogError(errorLog);
                    return errorLog;
                }
            }
        }
    }

    /// 群组解析器
    public class GroupParser : IParser
    {
        public string Encode(ChatMessage chatMessage)
        {
            switch (chatMessage.ChatData())
            {
                case IResChat resChat:
                {
                    // TODO ZZZ
                    return resChat.Message();
                }
                default:
                {
                    string errorLog = $"未支持的类型 => {chatMessage.ChatData().GetType()}";
                    Debug.LogError(errorLog);
                    return errorLog;
                }
            }
        }
    }
}

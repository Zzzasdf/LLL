using System.Collections.Generic;

namespace ChatModule
{
    /// 频道类型
    public enum EChatChannel
    {
        /// 无效
        Invalid = 0,
        /// 系统
        System = 1,
        /// 世界
        World = 2,
        /// 协会
        Union = 3,
        /// 队伍
        Team = 4,
        /// 私聊
        Private = 5,
        /// 群组
        Group = 6,
    }
    
    public partial class ChatManager
    {
        private Dictionary<EChatChannel, ChatChannelConfig> chatChannelConfigs;
        
        private void InitChannel()
        {
            chatChannelConfigs = new Dictionary<EChatChannel, ChatChannelConfig>
            {
                [EChatChannel.System] = new ChatChannelConfig(EChatSpaceStatus.Room),
                [EChatChannel.World] = new ChatChannelConfig(EChatSpaceStatus.Room),
                [EChatChannel.Union] = new ChatChannelConfig(EChatSpaceStatus.Room),
                [EChatChannel.Team] = new ChatChannelConfig(EChatSpaceStatus.Room),
                [EChatChannel.Private] = new ChatChannelConfig(EChatSpaceStatus.House),
                [EChatChannel.Group] = new ChatChannelConfig(EChatSpaceStatus.House),
            };
        }

        /// 获取所有频道类型
        public Dictionary<EChatChannel, ChatChannelConfig>.KeyCollection GetAllChatChannelConfig() => chatChannelConfigs.Keys;
        /// 获取频道类型配置
        public bool TryGetChatChannelConfig(EChatChannel eChatChannel, out ChatChannelConfig value) => chatChannelConfigs.TryGetValue(eChatChannel, out value);

        public class ChatChannelConfig
        {
            private EChatSpaceStatus eChatSpaceStatus;

            public ChatChannelConfig(EChatSpaceStatus eChatSpaceStatus)
            {
                this.eChatSpaceStatus = eChatSpaceStatus;
            }
        }
    }
}

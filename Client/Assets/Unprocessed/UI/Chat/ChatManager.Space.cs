using System.Collections.Generic;

namespace ChatModule
{
    /// 聊天空间状态
    public enum EChatSpaceStatus
    {
        Room = 1,
        House = 2,
    }
    
    /// 聊天室
    public enum EChatRoom
    {
        /// 系统
        System = 1,
        /// 世界
        World = 2,
        /// 协会
        Union = 3,
        /// 队伍
        Team = 4,
        /// 简易
        Simple = 5,
        /// 弹幕
        Barrage = 6,
    }
    /// 聊天屋
    public enum EChatHouse
    {
        /// 私聊
        Private = 1,
        /// 群组
        Group = 2,
    }
    
    public partial class ChatManager
    {
        /// 聊天室（含门牌）
        private Dictionary<EChatRoom, ChatRoom> rooms;
        /// 聊天屋（含大门，有多种同类型聊天室）
        private Dictionary<EChatHouse, ChatHouse> houses;
        
        private void InitConfigure()
        {
            // 聊天室定义
            {
                rooms = new Dictionary<EChatRoom, ChatRoom>();
                // 频道共用一个内容编辑贮藏器
                {
                    EditContentStorage channelEditContentStorage = new EditContentStorage();
                    // 系统
                    rooms.Add(EChatRoom.System,
                        new ChatRoom(new ChatRoomChannelTablet(EChatChannel.System), 50, new ChannelParser()));
                    // 世界
                    rooms.Add(EChatRoom.World, 
                        new ChatRoom(new ChatRoomChannelTablet(EChatChannel.World), 50, new ChannelParser())
                            .AttachUnit(channelEditContentStorage));
                    // 协会
                    rooms.Add(EChatRoom.Union,
                        new ChatRoom(new ChatRoomChannelTablet(EChatChannel.Union), 50, new ChannelParser())
                            .AttachUnit(channelEditContentStorage));
                    // 队伍
                    rooms.Add(EChatRoom.Team,
                        new ChatRoom(new ChatRoomChannelTablet(EChatChannel.Team), 50, new ChannelParser())
                            .AttachUnit(channelEditContentStorage));
                }
                
                // 简易
                rooms.Add(EChatRoom.Simple, 
                    new ChatRoom(null, 50, new SimpleParser()));
                // 弹幕
                rooms.Add(EChatRoom.Barrage, 
                    new ChatRoom(null, 1, new BarrageParser()));
            }
            
            // 聊天屋定义
            {
                houses = new Dictionary<EChatHouse, ChatHouse>();
                // 私聊
                houses.Add(EChatHouse.Private,
                    new ChatHouse(new ChatHouseChannelGate(EChatChannel.Private), 20, 
                        new ChatRoom(new ChatRoomPrivateTablet(), 50, new PrivateParser())
                            .AttachUnit(new EditContentStorage())));
                // 群组
                houses.Add(EChatHouse.Group,
                    new ChatHouse(new ChatHouseChannelGate(EChatChannel.Group), 20, 
                        new ChatRoom(new ChatRoomGroupTablet(), 50, new GroupParser())
                            .AttachUnit(new EditContentStorage())));
            }
        }

        /// 获取所有聊天室类型
        public Dictionary<EChatRoom, ChatRoom>.KeyCollection GetAllRoom() => rooms.Keys;
        /// 获取聊天室
        public bool TryGetRoom(EChatRoom eChatRoom, out ChatRoom value) => rooms.TryGetValue(eChatRoom, out value);

        /// 获取所有聊天屋类型
        public Dictionary<EChatHouse, ChatHouse>.KeyCollection GetAllHouse() => houses.Keys;
        /// 获取聊天屋
        public bool TryGetHouse(EChatHouse eChatHouse, out ChatHouse value) => houses.TryGetValue(eChatHouse, out value);
    }
}


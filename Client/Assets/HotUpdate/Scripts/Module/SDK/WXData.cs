public class WXData
{
    public string access_token; // 接口调用凭证
    public string expires_in; // access_token 接口调用凭证超时时间，单位（秒）
    public string refresh_token; // 用户刷新 access_token
    public string openid; // 授权用户唯一标识
    public string scope; // 用户授权的作用域，使用逗号（，）分隔
}

public class WXUserInfo
{
    public string openid; // 用户的标识，对当前开发者账号唯一
    public string nickname; // 用户昵称
    public int sex; // 用户性别，1(0)为男性，2(1)为女性
    public string province; // 用户个人资料填写的省份
    public string city; // 用户个人资料填写的城市
    public string country; // 国家，如中国为 CN
    public string headimgurl; // 用户头像。最后一个数值代表正方形头像大小（有 0、46、64、96、132 数值可选，0代表 640*640 正方形头像）
    public string[] privilege; // 用户特权信息，json 数值，如微信沃卡用户为（chinaunicom)
    public string unionid; // 用户统一标识，针对一个微信开放平台账号下的应用，同一用户的 unionid 是唯一的。
}

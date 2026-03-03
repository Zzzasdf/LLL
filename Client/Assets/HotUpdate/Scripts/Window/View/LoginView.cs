using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginView : ViewEntityBase<LoginViewModel>
{
    public Button btnLogin, btnQuit;
    public Image imgHead;
    public Text lbUsername, lbLog;
    public GameObject goLobby;
    
    private string APPID = "wx31d4a336465c38ab";
    private string SECRET = "8bd4006e8b65627ade116fc155f958fe";
    public override void InitUI(IViewCheck viewCheck)
    {
    }
    public override void DestroyUI()
    {
    }

    public override void BindUI()
    {
        btnLogin.onClick.AddListener(()=> viewModel.LoginCommand.Execute(null));
        btnQuit.onClick.AddListener(()=> viewModel.QuitCommand.Execute(null));
    }
    public override void UnBindUI()
    {
        btnLogin.onClick.RemoveAllListeners();
        btnQuit.onClick.RemoveAllListeners();
    }

    public void WXLoginCallBack(string str)
    {
        if (str != "用户取消" && str != "用户拒绝" && str != "其他错误")
        {
            LLogger.FrameLog($"微信登录成功，code是：{str}");
            lbLog.text += $"微信登录成功，code是：{str}\r\n";
            StartCoroutine(GetWXData(str)); // 获取微信数据
        }
        else
        {
            Debug.Log($"微信登录失败，code是：{str}");
        }
    }

    public IEnumerator GetWXData(string code)
    {
        string url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={APPID}&secret={SECRET}&code={code}&grant_type=authorization_code";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.isDone && request.error == null)
        {
            WXData wxdata = JsonUtility.FromJson<WXData>(request.downloadHandler.text);
            lbLog.text += $"wxdata:{wxdata.access_token}\r\nwxdata.openid:{wxdata.openid}\r\n";
            // 开始获取微信用户信息
            StartCoroutine(GetWXUserInfo(wxdata));
        }
    }

    public IEnumerator GetWXUserInfo(WXData wxdata)
    {
        if (wxdata != null)
        {
            string url_getuser = $"https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={wxdata.openid}&grant_type={wxdata.refresh_token}&refresh_token={wxdata.access_token}";
            UnityWebRequest request = UnityWebRequest.Get(url_getuser);
            yield return request.SendWebRequest();
            if (request.isDone && request.error == null)
            {
                WXUserInfo wxuserinfo = JsonUtility.FromJson<WXUserInfo>(request.downloadHandler.text);

                lbUsername.text = wxuserinfo.nickname;
                lbLog.text += $"\r\n姓名：{wxuserinfo.nickname} 性别：{wxuserinfo.sex.ToString()} 国家：{wxuserinfo.country} 省份：{wxuserinfo.province} 城市：{wxuserinfo.city} 统一标识：{wxuserinfo.unionid}";
                goLobby.SetActive(true);
                
                // 开始获得用户头像
                StartCoroutine(GetHeadImage(wxuserinfo));
            }
        }
    }

    public IEnumerator GetHeadImage(WXUserInfo wxUserInfo)
    {
        if (wxUserInfo != null)
        {
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(wxUserInfo.headimgurl))
            {
                yield return req.SendWebRequest();
                if (req.isDone && req.error == null)
                {
                    Texture2D texture2d = (req.downloadHandler as DownloadHandlerTexture).texture;
                    Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    imgHead.sprite = sprite;
                    goLobby.SetActive(true);
                }
                else
                {
                    LLogger.Log($"下载出错{req.responseCode}，{req.error}");
                    lbLog.text += $"\r\n下载出错{req.responseCode}，{req.error}";
                }
            }
        }
    }
}  

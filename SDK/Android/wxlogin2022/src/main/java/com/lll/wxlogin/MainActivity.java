package com.lll.wxlogin;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Bundle;
import android.widget.Toast;

import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXImageObject;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXTextObject;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {
    // APP_ID 替换为你的应用从官方网站申请到的合法appID
    private static final String APP_ID = "wx31d4a336465c38ab";
    // IWXAPI 是第三方app和微信通信的openApi接口
    private IWXAPI api;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        // 通过WXAPIFactory工厂，获取IWXAPI的实例
        api = WXAPIFactory.createWXAPI(this, APP_ID, true);

        // 将应用的appId注册到微信
        api.registerApp(APP_ID);
    }

    // 登录
    public void Login(){
        // send oauth request
        SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo"; // 只能填 snsapi_userinfo
        req.state = "wechat_sdk_demo_test";
        api.sendReq(req);
    }

    // 分享文字
    public void ShareText(String text, int mTargetScene) {
        Toast.makeText(this, "调用了ShareWeb方法", Toast.LENGTH_SHORT).show();
        //初始化一个 WXTextObject 对象，填写分享的文本内容
        WXTextObject textObj = new WXTextObject();
        textObj.text = text;

        //用 WXTextObject 对象初始化一个 WXMediaMessage 对象
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = textObj;
        msg.description = text;

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("text");
        req.message = msg;
        req.scene = mTargetScene;
        //调用api接口，发送数据到微信
        api.sendReq(req);
    }

    // 分享图片
    public void ShareImage(byte[] imageDate, byte[] thumbImageDate, int mTargetScene){
        Bitmap bmp = BitmapFactory.decodeResource(getResources(), R.drawable.send_img);

        //初始化 WXImageObject 和 WXMediaMessage 对象
        WXImageObject imgObj = new WXImageObject(bmp);
        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;

        //设置缩略图
        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
        bmp.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBmp, true);

        //构造一个Req
        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("img");
        req.message = msg;
        req.scene = mTargetScene;
        req.userOpenId = getOpenId();
        //调用api接口，发送数据到微信
        api.sendReq(req);
    }

    // 分享网页
    public void ShareWeb(String url, String title, String description, byte[] thumbImageDate, int scene) {

    }

    private String buildTransaction(final String type){

    }
}
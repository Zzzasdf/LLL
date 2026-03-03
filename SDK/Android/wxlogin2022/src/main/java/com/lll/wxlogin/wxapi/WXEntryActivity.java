package com.lll.wxlogin.wxapi;

import android.app.Activity;
import android.os.Bundle;
import android.widget.Toast;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.lll.wxlogin.R;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {

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

        api.handleIntent(getIntent(), this);
    }

    @Override
    public void onReq(BaseReq baseReq) {

    }

    @Override
    public void onResp(BaseResp baseResp) {
        if(baseResp.getType() == 1){ // 请求登录
            if(baseResp.errCode == BaseResp.ErrCode.ERR_OK){ // 用户同意
                UnityPlayer.UnitySendMessage("LoginView", "WXLoginCallBack", ((SendAuth.Resp)baseResp).code);
                Toast.makeText(this, "用户登录", Toast.LENGTH_SHORT).show();
            }
            else if(baseResp.errCode == BaseResp.ErrCode.ERR_USER_CANCEL) { // 用户取消
                UnityPlayer.UnitySendMessage("LoginView", "WxLoginCallBack", "用户取消");
                Toast.makeText(this, "用户取消", Toast.LENGTH_SHORT).show();
            }
            else if(baseResp.errCode == BaseResp.ErrCode.ERR_AUTH_DENIED){ // 用户拒绝
                UnityPlayer.UnitySendMessage("LoginView", "WXLoginCallBack", "用户拒绝");
                Toast.makeText(this, "用户拒绝", Toast.LENGTH_SHORT).show();
            }
            else {
                UnityPlayer.UnitySendMessage("LoginView", "WxLoginCallBack", "其他错误");
                Toast.makeText(this, "其他错误", Toast.LENGTH_SHORT).show();
            }
        }
        else if(baseResp.getType() == 2){ //

        }
        finish();
    }
}
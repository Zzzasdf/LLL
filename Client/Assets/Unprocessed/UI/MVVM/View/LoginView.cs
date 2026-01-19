using TMPro;
using UnityEngine.UI;

public class LoginView : ViewBase,
    IEvent<LoginModel>
{
    public TMP_InputField tmpUserName;
    public TMP_InputField tmpPassWord;
    public Button btnLogin;
    public Button btnRegister;

    protected override void OnSubscribe()
    {
        Events.Subscribe<LoginModel>(this);
    }

    void IEvent<LoginModel>.EventHandler(LoginModel data)
    {
        tmpUserName.text = data.UserName;
        tmpPassWord.text = data.PassWord;
    }
}

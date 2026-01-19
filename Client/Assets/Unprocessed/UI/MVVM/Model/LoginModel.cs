public class LoginModel
{
    public string UserName { get; private set; }
    public string PassWord { get; private set; }

    public void SetUserName(string userName)
    {
        this.UserName = userName;
    }
    public void SetPassWord(string passWord)
    {
        this.PassWord = passWord;
    }

    public void ReceiveServer()
    {
        SetUserName("1111");
        SetPassWord("2222");
        GameEntry.EventManager.FireNow(this);
    }

    public void SendServer()
    {
        
    }
}


public class LoginController
{
    private LoginView loginView;
    private LoginModel loginModel;
    
    private void Start()
    {
        ((IEvent<LoginModel>)loginView).EventHandler(loginModel);
    }

    void Update()
    {
        
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UnityEngine;

public partial class LoginViewModel : ObservableRecipient, IViewModel
{
    private AndroidJavaClass jc = null;
    private AndroidJavaObject jo = null;
    
    public LoginViewModel()
    {
        jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }
    
    [RelayCommand]
    private void Login()
    {
        jo.Call("login");
    }

    [RelayCommand]
    private void Quit()
    {
        Application.Quit();
    }
}

using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Cysharp.Threading.Tasks;

public partial class MainViewModel: ObservableRecipient, IRecipient<StringMessage>, IRecipient<IntMessage>
{
    [ObservableProperty]
    private int _score;

    [ObservableProperty]
    private int _score1;

    [RelayCommand]
    private void TakeDamage()
    {
        Score -= 10;
    }
    
    [RelayCommand]
    private void TakeDamage2()
    {
        IMessenger messenger = WeakReferenceMessenger.Default;
        messenger.Register<AsyncRequestMessage<string>>(this, (r, m) =>
        {
            m.Reply(Task.FromResult("das"));  
        });
        Score -= 10;
    }

    partial void OnScoreChanged(int value)
    {
        
    }
    
    public void Receive(StringMessage message)
    {
        
    }

    public void Receive(IntMessage message)
    {
    }
}

public class StringMessage
{
    
}

public class IntMessage
{
    
}

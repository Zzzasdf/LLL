using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Cysharp.Threading.Tasks;

public class ProcedureService: ObservableRecipient, IRecipient<GameStateMessage>
{
    public enum GameState
    {
        Preload,
        Start,
        SelectRole,
        CreateRole,
        Init,
        Main,
        Battle,
    }

    private Dictionary<GameState, IProcedure> procedures;

    public ProcedureService(Dictionary<GameState, IProcedure> procedures)
    {
        this.procedures = procedures;
        IsActive = true;
    }

    void IRecipient<GameStateMessage>.Receive(GameStateMessage message)
    {
        LLogger.FrameLog($"{nameof(ProcedureService)} run => {message.GameState}");
        procedures[message.GameState].Run().Forget();
    }
}

public class GameStateMessage
{
    public ProcedureService.GameState GameState { get; }
    public GameStateMessage(ProcedureService.GameState gameState)
    {
        GameState = gameState;
    }
}

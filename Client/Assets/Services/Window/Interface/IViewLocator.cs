public interface IViewLocator
{
    bool AnimationInit { get; set; }
    IAnimation EnterAnimation { get; set; }
    IAnimation ExitAnimation { get; set; }
}

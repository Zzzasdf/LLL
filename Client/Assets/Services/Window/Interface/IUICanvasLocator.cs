public interface IUICanvasLocator
{
    IViewService ViewService();
    ILayerLocator GetLayerLocator(ViewLayer viewLayer);
}

public interface IView
{
    void AddViewModel();
    void RemoveViewModel();

    void InitUI(IViewCheck viewCheck);
    void DestroyUI();
    void BindUI();
    void UnBindUI();
}

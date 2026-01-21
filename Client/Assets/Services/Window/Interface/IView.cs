using UnityEngine;

public interface IView
{
    public void Init(ViewLayer viewLayer, int uniqueId);

    public ViewLayer GetLayer();
    public int GetUniqueId();
    public GameObject GameObject();
}

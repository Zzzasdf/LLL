using UnityEngine;

public interface IWindow
{
    public void Init(WindowLayer windowLayer, int uniqueId);

    public WindowLayer GetLayer();
    public int GetUniqueId();
    public GameObject GameObject();
}

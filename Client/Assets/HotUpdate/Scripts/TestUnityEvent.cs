using UnityEngine;

public class TestUnityEvent : MonoBehaviour
{
    void Awake()
    {
        LLogger.Error("初始化");
    }

    private void OnDestroy()
    {
        LLogger.Error("销毁了");
    }
}

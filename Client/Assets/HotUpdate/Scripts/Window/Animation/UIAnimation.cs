using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class UIAnimation : MonoBehaviour, IAnimation
{
    [SerializeField]
    private UIAnimationType animationType;
    public UIAnimationType AnimationType
    {
        get => animationType;
        set => animationType = value;
    }

    public abstract UniTask DOPlayAsync();
}

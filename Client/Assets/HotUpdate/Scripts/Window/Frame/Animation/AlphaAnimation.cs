using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class AlphaAnimation : UIAnimation
{
    [Range(0f, 1f)] public float from = 1f;
    [Range(0f, 1f)] public float to = 1f;
    public float duration = 2f;

    private CanvasGroup _canvasGroup;
    private CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.alpha = 1f;
            }
            return _canvasGroup;
        }
    }

    public override async UniTask DOPlayAsync(CancellationToken token = default)
    {
        CanvasGroup.alpha = from;
        await CanvasGroup.DOFade(to, duration).ToUniTask(cancellationToken: token);
    }

    private void OnDestroy()
    {
        DOTween.Kill(_canvasGroup);
    }
}

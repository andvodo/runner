using DG.Tweening;
using TMPro;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    public void Setup()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true);
        DOTween.Sequence(_canvasGroup.DOFade(1f, 0.5f))
            .AppendCallback(() => _canvasGroup.blocksRaycasts = true)
            .Play();
    }

    public void Close()
    {
        _canvasGroup.blocksRaycasts = false;
        DOTween.Sequence(_canvasGroup.DOFade(0f, 0.5f))
            .AppendCallback(() => gameObject.SetActive(false)).Play();
    }
}

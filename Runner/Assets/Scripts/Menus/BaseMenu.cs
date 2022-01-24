using DG.Tweening;
using TMPro;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private bool _open;
    
    public void Setup()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        if (_open) return;
        
        _open = true;
        _canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true);
        DOTween.Sequence(_canvasGroup.DOFade(1f, 0.5f))
            .AppendCallback(() => _canvasGroup.blocksRaycasts = true)
            .Play();
    }

    public void Close()
    {
        if (!_open) return;
        
        _open = false;
        _canvasGroup.blocksRaycasts = false;
        DOTween.Sequence(_canvasGroup.DOFade(0f, 0.5f))
            .AppendCallback(() => gameObject.SetActive(false)).Play();
    }
}

using System;
using Assets.Scripts.Events;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Menus
{
    public class GameMenu : BaseMenu
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private CanvasGroup _brokenHeart;

        private Action _backCallback;
        private int _countNumber;
        public void Open()
        {
            base.Open();
            _brokenHeart.alpha = 0;
            _countNumber = 3;
            AnimateCountdown();
        }

        private void AnimateCountdown()
        {
            _label.text = _countNumber.ToString();
            _animator.SetTrigger("Count");
            _countNumber--;
            if (_countNumber > 0) Invoke(nameof(AnimateCountdown), 1f);
            else Invoke(nameof(Run), 1f);
        }

        private void Run()
        {
            EventController.CallEvent(new RunEvent());
        }
        
        public void Setup(Action backCallback)
        {
            base.Setup();
            _backCallback = backCallback;
        }

        public void Back()
        {
            CancelInvoke(nameof(Run));
            CancelInvoke(nameof(AnimateCountdown));
            Close();
            _backCallback?.Invoke();
        }
    }
}

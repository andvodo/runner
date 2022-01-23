using Assets.Scripts.Events;
using UnityEngine;
using TMPro;
using Event = Assets.Scripts.Events.Event;
using DG.Tweening;

namespace Assets.Scripts.Controllers
{
    public class HudController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _heartCounter;
        [SerializeField] private TMP_Text _coinCounter;
        [SerializeField] private RectTransform _heartImageHud;
        [SerializeField] private RectTransform _heartImageForAnimation;
        [SerializeField] private float _moveResourceToHudDuration;
        [SerializeField] private Animator _brokenHeartAnimator;

        private GameObject _player;
        private int _coinsNumber;
        private int _heartsNumber;

        public void Setup(GameObject player)
        {
            _player = player;
            EventController.Subscribe(
                new EventSubscription(HeartCollected, typeof(HeartCollectedEvent).ToString()));
            EventController.Subscribe(
                new EventSubscription(CoinCollected, typeof(CoinCollectedEvent).ToString()));
            EventController.Subscribe(
                new EventSubscription(LifeLost, typeof(LifeLostEvent).ToString()));
        }
        
        public void StartGame()
        {
            _heartCounter.text = "0";
            _coinCounter.text = "0";
            _heartsNumber = 0;
            _coinsNumber = 0;
        }

        private void HeartCollected(Event heartCollected)
        {
            _heartsNumber++;
            MoveToHud(_heartImageForAnimation, _heartImageHud.transform.position);
            _heartCounter.text = _heartsNumber.ToString();
        }

        private void CoinCollected(Event heartCollected)
        {
            _coinsNumber++;
            _coinCounter.text = _coinsNumber.ToString();
        }
        
        private void LifeLost(Event lifeLost)
        {
            _heartsNumber--;
            _heartCounter.text = _heartsNumber.ToString();
            _brokenHeartAnimator.SetTrigger("LifeLost");
        }
        
        private void MoveToHud(RectTransform resource, Vector3 endPosition)
        {
            resource.transform.position = Camera.main.WorldToScreenPoint(_player.transform.position);
            resource.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence()
                .Append(resource.transform.DOMove(endPosition, _moveResourceToHudDuration))
                .Append(resource.transform.DOScale(1.2f, 0.2f))
                .Append(resource.transform.DOScale(1f, 0.2f))
                .AppendCallback(() => resource.gameObject.SetActive(false));
            sequence.Play();
        }

        public void EndGame()
        {
            if (_coinsNumber <= PlayerPrefs.GetInt("highscore")) return;
            PlayerPrefs.SetInt("highscore", _coinsNumber);
        }
    }
}
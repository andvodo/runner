using System;
using Assets.Scripts.Events;
using UnityEngine;
using Event = Assets.Scripts.Events.Event;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private float _groundWidth = 10;
    
    private Rigidbody _cameraRB;
    private float _velocity;
    private const float _cameraStartZPosition = -10;
    private Action _cameraEndAnimationCallback;
    private EventSubscription _runSubscription;
    
    void Awake()
    {
        _cameraRB = GetComponent<Rigidbody>();
    }
    
    public void StartGame()
    {
        _runSubscription = EventController.Subscribe(new EventSubscription(Run, typeof(RunEvent).ToString()));
    }

    private void Run(Event runEvent)
    {
        _velocity = _player.Velocity;
        EventController.Unsubscribe(_runSubscription);
    }
    
    private void SetCameraStartPosition()
    {
        _cameraRB.position = new Vector3(_cameraRB.position.x, _cameraRB.position.y, _cameraStartZPosition);
    }
    
    public void EndGame(bool animate, Action callback = null)
    {
        if (!animate)
        {
            StopCamera();
            return;
        }
        _cameraEndAnimationCallback = callback;
        InvokeRepeating(nameof(SlowDown), 0, 0.1f);
    }

    // camera slow down animation on game end
    private void SlowDown()
    {
        _velocity -= 2;
        if (_velocity < 0)
        {
            _velocity = -2;
            CancelInvoke(nameof(SlowDown));
            Invoke(nameof(StopCamera), 2f);
        }
    }

    private void StopCamera()
    {
        _velocity = 0;
        _cameraEndAnimationCallback?.Invoke();
        _cameraEndAnimationCallback = null;
        SetCameraStartPosition();
    }
    
    
    private void FixedUpdate()
    {
        _cameraRB.position += Vector3.forward * Time.fixedDeltaTime * _velocity;
    }
}

using System;
using Assets.Scripts.Events;
using UnityEngine;
using Event = Assets.Scripts.Events.Event;

public class Player : MonoBehaviour
{
    [SerializeField] private float _sideCoordinate;
    [SerializeField] private float _velocity;

    private Rigidbody _playerRB;

    private EventSubscription _swipeRightSubscription;
    private EventSubscription _swipeLeftSubscription;
    private EventSubscription _runSubscription;

    private Position _currentPosition = Position.Center;
    private Position _goalPosition = Position.Center;
    private int _currentDirection;
    private int _lives;
    
    public float Velocity => _velocity;

    private Event _lastSwipeEvent;
    private double _lastSwipeTime;
    private bool _playing;

    public 
    void Awake()
    {
        _playerRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!_playing) return;
        
        CheckIfXPositionReached();
        
        if (GetSwipe(out int direction))
        {
            _currentDirection = direction;
            _lastSwipeEvent = null;
            _lastSwipeTime = 0;
        }
        
        Vector3 forwardMovement = Vector3.forward * Time.fixedDeltaTime * _velocity;
        Vector3 horizontalMovement = transform.right * _currentDirection * Time.fixedDeltaTime * _velocity;
        _playerRB.MovePosition(_playerRB.position + forwardMovement + horizontalMovement);
    }

    private void Run(Event runEvent)
    {
        Debug.Log("Player run");
        SwipeSubscribe();
        _playing = true;
        EventController.Unsubscribe(_runSubscription);
    }
    
    public void StartGame()
    {
        _currentPosition = Position.Center;
        _lives = 0;
        _playerRB.transform.position = new Vector3(0, 0.5f, 0);
        _runSubscription = EventController.Subscribe(new EventSubscription(Run, typeof(RunEvent).ToString()));
    }
    
    public void EndGame()
    {
        if (!_playing) return;
        _playing = false;
        SwipeUnsubscribe();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Cloud")
        {
            if (_lives > 0)
            {
                _lives--;
                EventController.CallEvent(new LifeLostEvent());
                
                return;
            }
            EndGame();
            EventController.CallEvent(new GameEndEvent());
        }

        if (other.tag == "Heart")
        {
            _lives++;
            EventController.CallEvent(new HeartCollectedEvent());
        } else if (other.tag == "Coin")
        {
            EventController.CallEvent(new CoinCollectedEvent());
        }
    }
    
    private void CheckIfXPositionReached()
    {
        if (_goalPosition == Position.Right && _playerRB.transform.position.x > _sideCoordinate
            || _goalPosition == Position.Left && _playerRB.transform.position.x < -_sideCoordinate
            || _goalPosition == Position.Center && Math.Abs(_playerRB.transform.position.x) < 0.1f)
        {
            _currentDirection = 0;
            _currentPosition = _goalPosition;
        }
    }

    private bool GetSwipe(out int direction)
    {
        direction = 0;
        
        if (!(Time.time - _lastSwipeTime < 0.5f) || _lastSwipeEvent == null) return false;

        if (_lastSwipeEvent is SwipeLeftEvent)
        {
            if (_currentPosition == Position.Left)
                return false;
            direction = -1;
            _goalPosition = _currentPosition == Position.Center ? Position.Left: Position.Center;
            return true;
        }
        if (_lastSwipeEvent is SwipeRightEvent)
        {
            if (_currentPosition == Position.Right)
                return false;
            _goalPosition = _currentPosition == Position.Center ? Position.Right : Position.Center;
            direction = 1;
            return true;
        }

        return false;
    }

    private void OnSwipe(Event swipeEvent)
    {
        _lastSwipeTime = Time.time;
        _lastSwipeEvent = swipeEvent;
    }
    
    private void SwipeSubscribe()
    {
        _swipeRightSubscription = EventController.Subscribe(
            new EventSubscription(OnSwipe, typeof(SwipeRightEvent).ToString()));
        _swipeLeftSubscription = EventController.Subscribe(
            new EventSubscription(OnSwipe, typeof(SwipeLeftEvent).ToString()));
    }

    private void SwipeUnsubscribe()
    {
        EventController.Unsubscribe(_swipeRightSubscription);
        EventController.Unsubscribe(_swipeLeftSubscription);
    }

    private enum Position
    {
        Left,
        Center,
        Right
    }
}

using System;
using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class SwipeController : MonoBehaviour
    {
        private const float TOUCH_DETECTION_DELTA = 100f;
        private bool _fingerDown;
        private Vector2 _startPosition;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                if (!_fingerDown)
                {
                    if (Input.touches[0].phase == TouchPhase.Began)
                    {
                        _startPosition = Input.touches[0].position;
                        _fingerDown = true;
                        return;
                    }
                }

                if (Input.touches[0].phase == TouchPhase.Ended)
                {
                    ProcessSwipe(Input.touches[0].position);
                }
            } else if (Input.GetMouseButtonDown(0) && !_fingerDown) {
                _startPosition = Input.mousePosition;
                _fingerDown = true;
            } else if (Input.GetMouseButtonUp(0) && _fingerDown)
            {
                ProcessSwipe(Input.mousePosition);
            }
        }

        private void ProcessSwipe(Vector2 currentPosition)
        {
            _fingerDown = false;
            
            double touchLengthX = Math.Abs(_startPosition.x - currentPosition.x);

            if (touchLengthX > TOUCH_DETECTION_DELTA)
            {
                if (_startPosition.x < currentPosition.x)
                {
                    EventController.CallEvent(new SwipeRightEvent());
                }
                else
                {
                    EventController.CallEvent(new SwipeLeftEvent());
                }
            }
        }
    }
}
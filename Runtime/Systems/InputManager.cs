using UnityEngine;
using System;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Systems
{
    public class InputManager : MonoBehaviour, IInputService
    {
        [Header("Settings")]
        [SerializeField] private float swipeThreshold = 50f;
        [SerializeField] private float holdThreshold = 0.5f;

        public bool IsInputEnabled { get; set; } = true;

        public event Action<Vector2> OnTap;
        public event Action<Vector2, Vector2> OnSwipe;
        public event Action<Vector2> OnDragStart;
        public event Action<Vector2> OnDrag;
        public event Action OnDragEnd;
        public event Action<Vector2> OnHoldStart;
        public event Action<Vector2> OnHold;
        public event Action OnHoldEnd;

        private Vector2 touchStartPos;
        private Vector2 touchCurrentPos;
        private bool isDragging;
        private bool isHolding;
        private float holdTimer;
        private bool holdTriggered;

        private void Update()
        {
            if (!IsInputEnabled) return;

            HandleInput();
        }

        private void HandleInput()
        {
            // Mouse/Touch input
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                touchCurrentPos = touchStartPos;
                holdTimer = 0f;
                holdTriggered = false;
            }

            if (Input.GetMouseButton(0))
            {
                touchCurrentPos = Input.mousePosition;
                float distance = Vector2.Distance(touchStartPos, touchCurrentPos);

                // Check for drag
                if (!isDragging && distance > swipeThreshold * 0.5f)
                {
                    isDragging = true;
                    OnDragStart?.Invoke(touchStartPos);
                }

                if (isDragging)
                {
                    OnDrag?.Invoke(touchCurrentPos);
                }

                // Check for hold
                if (!isDragging && !holdTriggered)
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= holdThreshold)
                    {
                        isHolding = true;
                        holdTriggered = true;
                        OnHoldStart?.Invoke(touchStartPos);
                    }
                }

                if (isHolding)
                {
                    OnHold?.Invoke(touchCurrentPos);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                touchCurrentPos = Input.mousePosition;
                float distance = Vector2.Distance(touchStartPos, touchCurrentPos);

                if (isDragging)
                {
                    OnDragEnd?.Invoke();
                    isDragging = false;
                }
                else if (isHolding)
                {
                    OnHoldEnd?.Invoke();
                    isHolding = false;
                }
                else if (distance > swipeThreshold)
                {
                    // Swipe
                    Vector2 direction = (touchCurrentPos - touchStartPos).normalized;
                    OnSwipe?.Invoke(touchStartPos, direction);
                }
                else
                {
                    // Tap
                    OnTap?.Invoke(touchCurrentPos);
                }

                holdTimer = 0f;
                holdTriggered = false;
            }
        }

        public Vector2 GetInputPosition()
        {
            return Input.mousePosition;
        }

        public Vector2 GetDragDelta()
        {
            return touchCurrentPos - touchStartPos;
        }

        public float GetHorizontalAxis()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVerticalAxis()
        {
            return Input.GetAxis("Vertical");
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
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
        private bool pointerDown;

        private void Update()
        {
            if (!IsInputEnabled) return;

            HandleInput();
        }

        private void HandleInput()
        {
            var pointer = Pointer.current;
            if (pointer == null) return;

            bool pressed = pointer.press.isPressed;
            bool justPressed = pointer.press.wasPressedThisFrame;
            bool justReleased = pointer.press.wasReleasedThisFrame;
            Vector2 position = pointer.position.ReadValue();

            if (justPressed)
            {
                pointerDown = true;
                touchStartPos = position;
                touchCurrentPos = touchStartPos;
                holdTimer = 0f;
                holdTriggered = false;
            }

            if (pressed && pointerDown)
            {
                touchCurrentPos = position;
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

            if (justReleased && pointerDown)
            {
                pointerDown = false;
                touchCurrentPos = position;
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
            var pointer = Pointer.current;
            return pointer != null ? pointer.position.ReadValue() : Vector2.zero;
        }

        public Vector2 GetDragDelta()
        {
            return touchCurrentPos - touchStartPos;
        }

        public float GetHorizontalAxis()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return 0f;
            float val = 0f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) val += 1f;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) val -= 1f;
            return val;
        }

        public float GetVerticalAxis()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return 0f;
            float val = 0f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) val += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) val -= 1f;
            return val;
        }
    }
}

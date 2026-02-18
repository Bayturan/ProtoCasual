using UnityEngine;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Mechanics
{
    public class TapToJumpMechanic : Systems.MechanicBase
    {
        public override string MechanicName => "Tap To Jump";

        [SerializeField] private Transform targetTransform;
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float gravity = -20f;

        private IInputService inputService;
        private float verticalVelocity;
        private bool isGrounded = true;

        protected override void OnInitialize()
        {
            inputService = ServiceLocator.Get<IInputService>();
        }

        protected override void OnEnable()
        {
            if (inputService != null)
            {
                inputService.OnTap += HandleTap;
            }
        }

        protected override void OnDisable()
        {
            if (inputService != null)
            {
                inputService.OnTap -= HandleTap;
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (targetTransform == null) return;

            // Apply gravity
            verticalVelocity += gravity * deltaTime;

            Vector3 position = targetTransform.position;
            position.y += verticalVelocity * deltaTime;

            // Ground check
            if (position.y <= 0f)
            {
                position.y = 0f;
                verticalVelocity = 0f;
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }

            targetTransform.position = position;
        }

        private void HandleTap(Vector2 position)
        {
            if (isGrounded && targetTransform != null)
            {
                verticalVelocity = jumpForce;
            }
        }

        protected override void OnCleanup()
        {
            verticalVelocity = 0f;
            isGrounded = true;
        }
    }
}

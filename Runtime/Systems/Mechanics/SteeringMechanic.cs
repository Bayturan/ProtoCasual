using UnityEngine;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Mechanics
{
    public class SteeringMechanic : Systems.MechanicBase
    {
        public override string MechanicName => "Steering";

        [SerializeField] private Transform targetTransform;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float turnSpeed = 180f;
        [SerializeField] private float acceleration = 5f;

        private IInputService inputService;
        private float currentSpeed;

        protected override void OnMechanicInitialize()
        {
            inputService = ServiceLocator.Get<IInputService>();
            currentSpeed = 0f;
        }

        protected override void OnMechanicEnable()
        {
            currentSpeed = 0f;
        }

        protected override void OnMechanicDisable()
        {
            currentSpeed = 0f;
        }

        protected override void OnMechanicUpdate(float deltaTime)
        {
            if (targetTransform == null || inputService == null) return;

            // Accelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * deltaTime);

            // Get input
            float horizontalInput = inputService.GetHorizontalAxis();
            float verticalInput = inputService.GetVerticalAxis();

            // Move forward
            targetTransform.position += targetTransform.forward * verticalInput * currentSpeed * deltaTime;

            // Turn
            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                float turnAmount = horizontalInput * turnSpeed * deltaTime;
                targetTransform.Rotate(0, turnAmount, 0);
            }
        }

        protected override void OnMechanicCleanup()
        {
            currentSpeed = 0f;
        }
    }
}

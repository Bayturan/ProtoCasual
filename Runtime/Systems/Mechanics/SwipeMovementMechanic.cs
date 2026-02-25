using UnityEngine;
using ProtoCasual.Core.Bootstrap;
using ProtoCasual.Core.Interfaces;

namespace ProtoCasual.Core.Mechanics
{
    public class SwipeMovementMechanic : Systems.MechanicBase
    {
        public override string MechanicName => "Swipe Movement";

        [SerializeField] private Transform targetTransform;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float horizontalRange = 5f;

        private IInputService inputService;
        private Vector3 startPosition;

        protected override void OnMechanicInitialize()
        {
            inputService = ServiceLocator.Get<IInputService>();
            
            if (targetTransform != null)
            {
                startPosition = targetTransform.position;
            }
        }

        protected override void OnMechanicEnable()
        {
            if (inputService != null)
            {
                inputService.OnDrag += HandleDrag;
            }
        }

        protected override void OnMechanicDisable()
        {
            if (inputService != null)
            {
                inputService.OnDrag -= HandleDrag;
            }
        }

        protected override void OnMechanicUpdate(float deltaTime)
        {
            if (targetTransform == null) return;

            // Forward movement
            targetTransform.position += Vector3.forward * moveSpeed * deltaTime;
        }

        private void HandleDrag(Vector2 position)
        {
            if (targetTransform == null) return;

            // Convert screen position to world horizontal movement
            float screenCenter = Screen.width * 0.5f;
            float offset = (position.x - screenCenter) / screenCenter;
            float targetX = Mathf.Clamp(offset * horizontalRange, -horizontalRange, horizontalRange);

            Vector3 newPos = targetTransform.position;
            newPos.x = Mathf.Lerp(newPos.x, targetX, Time.deltaTime * 10f);
            targetTransform.position = newPos;
        }

        protected override void OnMechanicCleanup()
        {
            if (targetTransform != null)
            {
                targetTransform.position = startPosition;
            }
        }
    }
}

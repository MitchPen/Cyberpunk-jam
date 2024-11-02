using UnityEngine;

namespace Main.Scripts
{
    public class EnemyMovementComponent : MonoBehaviour
    {
        public float moveSpeed = 2f; // Speed of the enemy
        public float movementRange = 1f; // Range for random movement
        public float changeDirectionTime = 2f; // Time before changing direction

        private Vector3 targetPosition;
        private float timer;

        void Start()
        {
            // Set the initial target position
            SetNewTargetPosition();
        }

        void Update()
        {
            // Move towards the target position
            MoveTowardsTarget();

            // Check if it's time to change direction
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                SetNewTargetPosition();
                timer = 0f;
            }
        }

        void MoveTowardsTarget()
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        void SetNewTargetPosition()
        {
            Vector3 randomDirection;
            bool validPositionFound = false;
            int attempts = 0;

            // Try to find a valid position without exceeding attempts
            while (!validPositionFound && attempts < 10)
            {
                attempts++;
                randomDirection = new Vector3(
                    Random.Range(-movementRange, movementRange),
                    Random.Range(-movementRange, movementRange),
                    0 // Keep Z-axis constant if 2D
                );

                Vector3 potentialPosition = transform.position + randomDirection;

                // Check for collisions
                if (!Physics.CheckSphere(potentialPosition, 3f)) // Adjust the radius as needed
                {
                    targetPosition = potentialPosition;
                    validPositionFound = true;
                }
            }

            // If no valid position is found after several attempts, keep the current target position
            if (!validPositionFound)
            {
                targetPosition = transform.position; // Stay in place if no valid move
            }
        }
    }
}

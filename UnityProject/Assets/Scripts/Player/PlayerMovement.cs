using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Handles 3-lane horizontal switching, jump physics, sliding height adjustment,
    /// and dynamic forward speed acceleration.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Lane Settings")]
        public float laneWidth = 2.5f;
        public float laneChangeSpeed = 15f;
        public int currentLane = 1; // 0 = Left (-2.5), 1 = Center (0), 2 = Right (2.5)

        [Header("Running Speed Settings")]
        public float forwardSpeed = 16f;
        public float maximumSpeed = 35f;
        public float speedAcceleration = 0.15f;

        [Header("Jump Physics")]
        public float jumpForce = 12f;
        public float gravity = -32f;
        private float verticalVelocity = 0f;
        public bool isGrounded = true;

        [Header("Slide Settings")]
        public float slideDuration = 0.85f;
        public bool isSliding = false;
        private float slideTimer = 0f;

        [Header("Collider Settings")]
        public CapsuleCollider playerCollider;
        private Vector3 originalColliderCenter;
        private float originalColliderHeight;

        private Vector3 targetPosition;
        private bool isMoving = true;

        private void Start()
        {
            if (playerCollider == null) playerCollider = GetComponent<CapsuleCollider>();
            if (playerCollider != null)
            {
                originalColliderCenter = playerCollider.center;
                originalColliderHeight = playerCollider.height;
            }
            ResetPosition();
        }

        public void ResetPosition()
        {
            currentLane = 1;
            targetPosition = new Vector3(0f, transform.position.y, transform.position.z);
            transform.position = targetPosition;
            verticalVelocity = 0f;
            isGrounded = true;
            isSliding = false;
            isMoving = true;
            forwardSpeed = 16f;
        }

        private void Update()
        {
            if (!isMoving || PlayerController.Instance == null || !PlayerController.Instance.isAlive)
                return;

            // Forward speed scaling over distance
            forwardSpeed = Mathf.Min(maximumSpeed, forwardSpeed + speedAcceleration * Time.deltaTime);

            // Horizontal Lane lerping
            float targetX = (currentLane - 1) * laneWidth;
            Vector3 currentPos = transform.position;
            currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * laneChangeSpeed);

            // Vertical Jump Physics
            if (isGrounded)
            {
                verticalVelocity = -0.5f; // Small grounding offset
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            currentPos.y += verticalVelocity * Time.deltaTime;

            // Fall Detection trigger
            if (currentPos.y < -5f && PlayerController.Instance != null && PlayerController.Instance.isAlive)
            {
                PlayerController.Instance.Die();
                return;
            }

            if (currentPos.y <= 0f && isGrounded)
            {
                currentPos.y = 0f;
                verticalVelocity = 0f;
                isGrounded = true;
            }

            // Handle Slide Duration
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0f)
                {
                    StopSlide();
                }
            }

            // Apply forward velocity
            currentPos.z += forwardSpeed * PlayerController.Instance.currentSpeedMultiplier * Time.deltaTime;

            transform.position = currentPos;
        }

        public void ChangeLane(int direction)
        {
            // direction: -1 for left, +1 for right
            int newLane = Mathf.Clamp(currentLane + direction, 0, 2);
            currentLane = newLane;
        }

        public bool Jump()
        {
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
                if (isSliding) StopSlide();
                return true;
            }
            return false;
        }

        public bool Slide()
        {
            if (!isSliding)
            {
                isSliding = true;
                slideTimer = slideDuration;

                if (playerCollider != null)
                {
                    playerCollider.height = originalColliderHeight * 0.5f;
                    playerCollider.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y * 0.5f, originalColliderCenter.z);
                }

                if (!isGrounded)
                {
                    // Fast downward drop if sliding mid-air
                    verticalVelocity = -20f;
                }
                return true;
            }
            return false;
        }

        private void StopSlide()
        {
            isSliding = false;
            if (playerCollider != null)
            {
                playerCollider.height = originalColliderHeight;
                playerCollider.center = originalColliderCenter;
            }
        }

        public void StopMovement()
        {
            isMoving = false;
        }

        public void ResumeMovement()
        {
            isMoving = true;
        }
    }
}

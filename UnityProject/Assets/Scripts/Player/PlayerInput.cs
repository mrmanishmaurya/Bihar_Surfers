using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Processes mobile touch swipe gestures (Swipe Left/Right/Up/Down)
    /// and keyboard arrow/WASD input for PC testing.
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        [Header("Swipe Detection Thresholds")]
        public float minSwipeDistance = 50f;
        public float maxSwipeTime = 0.5f;

        private Vector2 swipeStartPosition;
        private float swipeStartTime;
        private bool isSwiping = false;

        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (playerController == null || !playerController.isAlive) return;

            ProcessKeyboardInput();
            ProcessTouchInput();
            ProcessMouseInput();
        }

        private void ProcessMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSwiping = true;
                swipeStartPosition = Input.mousePosition;
                swipeStartTime = Time.time;
            }
            else if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                float swipeTime = Time.time - swipeStartTime;
                Vector2 swipeDelta = (Vector2)Input.mousePosition - swipeStartPosition;

                if (swipeTime <= maxSwipeTime && swipeDelta.magnitude >= minSwipeDistance)
                {
                    DetectSwipeDirection(swipeDelta);
                }
                isSwiping = false;
            }
        }

        private void ProcessKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerController.OnLaneChange(-1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                playerController.OnLaneChange(1);
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
            {
                playerController.OnJump();
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                playerController.OnSlide();
            }
        }

        private void ProcessTouchInput()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isSwiping = true;
                    swipeStartPosition = touch.position;
                    swipeStartTime = Time.time;
                    break;

                case TouchPhase.Ended:
                    if (!isSwiping) return;

                    float swipeTime = Time.time - swipeStartTime;
                    Vector2 swipeDelta = touch.position - swipeStartPosition;

                    if (swipeTime <= maxSwipeTime && swipeDelta.magnitude >= minSwipeDistance)
                    {
                        DetectSwipeDirection(swipeDelta);
                    }
                    isSwiping = false;
                    break;

                case TouchPhase.Canceled:
                    isSwiping = false;
                    break;
            }
        }

        private void DetectSwipeDirection(Vector2 swipeDelta)
        {
            swipeDelta.Normalize();

            // Horizontal Swipe
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                if (swipeDelta.x > 0)
                {
                    playerController.OnLaneChange(1); // Swipe Right
                }
                else
                {
                    playerController.OnLaneChange(-1); // Swipe Left
                }
            }
            // Vertical Swipe
            else
            {
                if (swipeDelta.y > 0)
                {
                    playerController.OnJump(); // Swipe Up
                }
                else
                {
                    playerController.OnSlide(); // Swipe Down
                }
            }
        }
    }
}

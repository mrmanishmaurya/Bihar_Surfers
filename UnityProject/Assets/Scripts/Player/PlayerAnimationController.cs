using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Connects PlayerController logic to Unity's Animator state machine
    /// for smooth transitions between Idle, Run, Jump, Slide, Hit, and Death.
    /// </summary>
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;

        // Animator Parameter Hashes
        private static readonly int IsRunningHash = Animator.StringToHash("IsRunning");
        private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
        private static readonly int FallTriggerHash = Animator.StringToHash("Fall");
        private static readonly int SlideTriggerHash = Animator.StringToHash("Slide");
        private static readonly int HitTriggerHash = Animator.StringToHash("Hit");
        private static readonly int DeathTriggerHash = Animator.StringToHash("Death");

        private void Awake()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        public void PlayRun()
        {
            if (animator != null)
            {
                animator.SetBool(IsRunningHash, true);
            }
        }

        public void PlayJump()
        {
            if (animator != null)
            {
                animator.SetTrigger(JumpTriggerHash);
            }
        }

        public void PlayFall()
        {
            if (animator != null)
            {
                animator.SetTrigger(FallTriggerHash);
            }
        }

        public void PlaySlide()
        {
            if (animator != null)
            {
                animator.SetTrigger(SlideTriggerHash);
            }
        }

        public void PlayHit()
        {
            if (animator != null)
            {
                animator.SetTrigger(HitTriggerHash);
            }
        }

        public void PlayDeath()
        {
            if (animator != null)
            {
                animator.SetBool(IsRunningHash, false);
                animator.SetTrigger(DeathTriggerHash);
            }
        }
    }
}

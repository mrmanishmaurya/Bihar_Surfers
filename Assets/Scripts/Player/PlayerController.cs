using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Master controller for the Bihar Surfers player character.
    /// Coordinates movement, input, collisions, powerups, and animations.
    /// </summary>
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerAnimationController))]
    [RequireComponent(typeof(PlayerCollision))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Sub-Components")]
        public PlayerMovement movement;
        public PlayerInput input;
        public PlayerAnimationController animationController;
        public PlayerCollision collision;

        [Header("State Flags")]
        public bool isAlive = true;
        public bool isShielded = false;
        public bool isMagnetActive = false;
        public bool isInvincible = false;
        public float currentSpeedMultiplier = 1.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            movement = GetComponent<PlayerMovement>();
            input = GetComponent<PlayerInput>();
            animationController = GetComponent<PlayerAnimationController>();
            collision = GetComponent<PlayerCollision>();

            EnsurePlayerVisuals();
        }

        private void EnsurePlayerVisuals()
        {
            // 1. Remove direct MeshFilter / MeshRenderer from main player root if attached to prevent capsule render
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null) Destroy(rootRenderer);
            MeshFilter rootFilter = GetComponent<MeshFilter>();
            if (rootFilter != null) Destroy(rootFilter);

            // 2. Remove old primitive capsule or temporary visual children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.name.Contains("Capsule") || child.name.Contains("VisualPrimitive") || child.name.Contains("CharacterVisuals"))
                {
                    Destroy(child.gameObject);
                }
            }

            // Check if CharacterVisual container already exists
            Transform existingHolder = transform.Find("CharacterVisual");
            if (existingHolder != null && existingHolder.transform.Find("Bihar_Surfers_Boy") != null)
            {
                return;
            }

            // Create CharacterVisual parent container under Player
            GameObject characterVisualHolder = new GameObject("CharacterVisual");
            characterVisualHolder.transform.SetParent(transform, false);
            characterVisualHolder.transform.localPosition = Vector3.zero;
            characterVisualHolder.transform.localRotation = Quaternion.identity;
            characterVisualHolder.transform.localScale = Vector3.one;

            // Attempt to load imported 3D Character Model (Bihar_Surfers_Boy) from Resources or AssetDatabase
            GameObject characterPrefab = Resources.Load<GameObject>("Bihar_Surfers_Boy");
#if UNITY_EDITOR
            if (characterPrefab == null)
            {
                characterPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/BiharSurfersBoy/Bihar_Surfers_Boy.glb");
            }
            if (characterPrefab == null)
            {
                characterPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Player/Bihar_Surfers_Boy.glb");
            }
#endif

            if (characterPrefab != null)
            {
                GameObject modelInstance = Instantiate(characterPrefab, characterVisualHolder.transform);
                modelInstance.name = "Bihar_Surfers_Boy";
                modelInstance.transform.localPosition = Vector3.zero;
                modelInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                modelInstance.transform.localScale = Vector3.one;

                Animator modelAnimator = modelInstance.GetComponentInChildren<Animator>();
                if (modelAnimator != null && animationController != null)
                {
                    animationController.animator = modelAnimator;
                }
                return;
            }

            // Fallback: Construct Multi-Part 3D Human Character Visuals (Head, Torso, Arms, Legs)
            GameObject characterHolder = new GameObject("CharacterVisuals");
            characterHolder.transform.SetParent(transform, false);
            characterHolder.transform.localPosition = Vector3.zero;

            // Head (Sphere)
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(characterHolder.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            head.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            Environment.MaterialFactory.ApplyMaterial(head, Environment.BiharMaterialType.PlayerSkin);
            Destroy(head.GetComponent<Collider>());

            // Torso / Body (Cube - Bihari Red Gamcha / Kurta)
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Torso";
            body.transform.SetParent(characterHolder.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            body.transform.localScale = new Vector3(0.55f, 0.7f, 0.32f);
            Environment.MaterialFactory.ApplyMaterial(body, Environment.BiharMaterialType.PlayerBody);
            Destroy(body.GetComponent<Collider>());

            // Left Arm (Cylinder)
            GameObject armL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            armL.name = "Arm_Left";
            armL.transform.SetParent(characterHolder.transform, false);
            armL.transform.localPosition = new Vector3(-0.38f, 0.95f, 0f);
            armL.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
            Environment.MaterialFactory.ApplyMaterial(armL, Environment.BiharMaterialType.PlayerSkin);
            Destroy(armL.GetComponent<Collider>());

            // Right Arm (Cylinder)
            GameObject armR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            armR.name = "Arm_Right";
            armR.transform.SetParent(characterHolder.transform, false);
            armR.transform.localPosition = new Vector3(0.38f, 0.95f, 0f);
            armR.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
            Environment.MaterialFactory.ApplyMaterial(armR, Environment.BiharMaterialType.PlayerSkin);
            Destroy(armR.GetComponent<Collider>());

            // Left Leg (Cylinder - Dark Trousers)
            GameObject legL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legL.name = "Leg_Left";
            legL.transform.SetParent(characterHolder.transform, false);
            legL.transform.localPosition = new Vector3(-0.16f, 0.32f, 0f);
            legL.transform.localScale = new Vector3(0.15f, 0.32f, 0.15f);
            Environment.MaterialFactory.ApplyMaterial(legL, Environment.BiharMaterialType.SteelRail);
            Destroy(legL.GetComponent<Collider>());

            // Right Leg (Cylinder - Dark Trousers)
            GameObject legR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legR.name = "Leg_Right";
            legR.transform.SetParent(characterHolder.transform, false);
            legR.transform.localPosition = new Vector3(0.16f, 0.32f, 0f);
            legR.transform.localScale = new Vector3(0.15f, 0.32f, 0.15f);
            Environment.MaterialFactory.ApplyMaterial(legR, Environment.BiharMaterialType.SteelRail);
            Destroy(legR.GetComponent<Collider>());
        }

        private void Start()
        {
            ResetPlayerState();
        }

        public void ResetPlayerState()
        {
            isAlive = true;
            isShielded = false;
            isMagnetActive = false;
            isInvincible = false;
            currentSpeedMultiplier = 1.0f;

            if (movement != null) movement.ResetPosition();
            if (animationController != null) animationController.PlayRun();
        }

        public void ResetPlayer() => ResetPlayerState();

        public void OnLaneChange(int targetLane)
        {
            if (!isAlive) return;
            movement.ChangeLane(targetLane);
        }

        public void OnJump()
        {
            if (!isAlive) return;
            if (movement.Jump())
            {
                animationController.PlayJump();
                Core.AudioManager.Instance?.PlaySFX("Jump");
            }
        }

        public void OnSlide()
        {
            if (!isAlive) return;
            if (movement.Slide())
            {
                animationController.PlaySlide();
                Core.AudioManager.Instance?.PlaySFX("Slide");
            }
        }

        public void OnHitObstacle()
        {
            if (isInvincible) return;

            if (isShielded)
            {
                isShielded = false;
                Core.AudioManager.Instance?.PlaySFX("ShieldBreak");
                // Trigger shield hit particle/feedback
                return;
            }

            Die();
        }

        public void Die()
        {
            if (!isAlive) return;
            isAlive = false;

            movement.StopMovement();
            animationController.PlayDeath();
            Core.AudioManager.Instance?.PlaySFX("Collision");
            Core.AudioManager.Instance?.PlaySFX("GameOver");

            Core.GameManager.Instance?.TriggerGameOver();
        }

        public void Revive()
        {
            isAlive = true;
            isInvincible = true;
            movement.ResumeMovement();
            animationController.PlayRun();
            Invoke(nameof(DisableInvincibility), 3.0f);
        }

        private void DisableInvincibility()
        {
            isInvincible = false;
        }
    }
}

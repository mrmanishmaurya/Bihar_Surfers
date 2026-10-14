using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Master controller for the Bihar Surfers player character.
    /// Coordinates movement, input, collisions, powerups, animations,
    /// and dynamic 3D character visual customization across all 5 playable characters.
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

        [Header("Visual Container")]
        private GameObject characterVisualHolder;
        private Renderer torsoRenderer;
        private Renderer gamchaRenderer;

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

        private void Start()
        {
            ResetPlayerState();

            if (CharacterManager.Instance != null)
            {
                ApplyCharacterVisuals(CharacterManager.Instance.GetSelectedCharacter());
            }
        }

        public void EnsurePlayerVisuals()
        {
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null) Destroy(rootRenderer);
            MeshFilter rootFilter = GetComponent<MeshFilter>();
            if (rootFilter != null) Destroy(rootFilter);

            Transform existing = transform.Find("CharacterVisual");
            if (existing != null)
            {
                Destroy(existing.gameObject);
            }

            characterVisualHolder = new GameObject("CharacterVisual");
            characterVisualHolder.transform.SetParent(transform, false);
            characterVisualHolder.transform.localPosition = Vector3.zero;
            characterVisualHolder.transform.localRotation = Quaternion.identity;
            characterVisualHolder.transform.localScale = Vector3.one;

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

            BuildStylizedRunnerMesh(characterVisualHolder.transform);
        }

        private void BuildStylizedRunnerMesh(Transform parent)
        {
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(parent, false);
            head.transform.localPosition = new Vector3(0f, 1.55f, 0f);
            head.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            Environment.MaterialFactory.ApplyMaterial(head, Environment.BiharMaterialType.PlayerSkin);
            Destroy(head.GetComponent<Collider>());

            GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hair.name = "Hair";
            hair.transform.SetParent(head.transform, false);
            hair.transform.localPosition = new Vector3(0f, 0.12f, -0.05f);
            hair.transform.localScale = new Vector3(1.02f, 0.75f, 1.05f);
            Environment.MaterialFactory.ApplyMaterial(hair, Environment.BiharMaterialType.SteelRail);
            Destroy(hair.GetComponent<Collider>());

            GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
            torso.name = "Torso";
            torso.transform.SetParent(parent, false);
            torso.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            torso.transform.localScale = new Vector3(0.55f, 0.70f, 0.32f);
            Environment.MaterialFactory.ApplyMaterial(torso, Environment.BiharMaterialType.PlayerBody);
            torsoRenderer = torso.GetComponent<Renderer>();
            Destroy(torso.GetComponent<Collider>());

            GameObject gamcha = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gamcha.name = "Gamcha";
            gamcha.transform.SetParent(parent, false);
            gamcha.transform.localPosition = new Vector3(0.08f, 1.15f, 0.05f);
            gamcha.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            gamcha.transform.localScale = new Vector3(0.62f, 0.16f, 0.38f);
            Environment.MaterialFactory.ApplyMaterial(gamcha, Environment.BiharMaterialType.Barricade);
            gamchaRenderer = gamcha.GetComponent<Renderer>();
            Destroy(gamcha.GetComponent<Collider>());

            GameObject armL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            armL.name = "Arm_Left";
            armL.transform.SetParent(parent, false);
            armL.transform.localPosition = new Vector3(-0.38f, 0.95f, 0f);
            armL.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
            Environment.MaterialFactory.ApplyMaterial(armL, Environment.BiharMaterialType.PlayerSkin);
            Destroy(armL.GetComponent<Collider>());

            GameObject armR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            armR.name = "Arm_Right";
            armR.transform.SetParent(parent, false);
            armR.transform.localPosition = new Vector3(0.38f, 0.95f, 0f);
            armR.transform.localScale = new Vector3(0.12f, 0.32f, 0.12f);
            Environment.MaterialFactory.ApplyMaterial(armR, Environment.BiharMaterialType.PlayerSkin);
            Destroy(armR.GetComponent<Collider>());

            GameObject legL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legL.name = "Leg_Left";
            legL.transform.SetParent(parent, false);
            legL.transform.localPosition = new Vector3(-0.16f, 0.32f, 0f);
            legL.transform.localScale = new Vector3(0.15f, 0.32f, 0.15f);
            Environment.MaterialFactory.ApplyMaterial(legL, Environment.BiharMaterialType.SteelRail);
            Destroy(legL.GetComponent<Collider>());

            GameObject legR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            legR.name = "Leg_Right";
            legR.transform.SetParent(parent, false);
            legR.transform.localPosition = new Vector3(0.16f, 0.32f, 0f);
            legR.transform.localScale = new Vector3(0.15f, 0.32f, 0.15f);
            Environment.MaterialFactory.ApplyMaterial(legR, Environment.BiharMaterialType.SteelRail);
            Destroy(legR.GetComponent<Collider>());

            GameObject shoeL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shoeL.name = "Shoe_Left";
            shoeL.transform.SetParent(parent, false);
            shoeL.transform.localPosition = new Vector3(-0.16f, 0.05f, 0.06f);
            shoeL.transform.localScale = new Vector3(0.16f, 0.10f, 0.30f);
            Environment.MaterialFactory.ApplyMaterial(shoeL, Environment.BiharMaterialType.Platform);
            Destroy(shoeL.GetComponent<Collider>());

            GameObject shoeR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shoeR.name = "Shoe_Right";
            shoeR.transform.SetParent(parent, false);
            shoeR.transform.localPosition = new Vector3(0.16f, 0.05f, 0.06f);
            shoeR.transform.localScale = new Vector3(0.16f, 0.10f, 0.30f);
            Environment.MaterialFactory.ApplyMaterial(shoeR, Environment.BiharMaterialType.Platform);
            Destroy(shoeR.GetComponent<Collider>());
        }

        public void ApplyCharacterVisuals(Data.CharacterData charData)
        {
            if (charData == null) return;

            if (torsoRenderer != null)
            {
                Material m = new Material(torsoRenderer.sharedMaterial);
                m.color = charData.primaryThemeColor;
                torsoRenderer.material = m;
            }

            if (gamchaRenderer != null)
            {
                Material gm = new Material(gamchaRenderer.sharedMaterial);
                gm.color = Color.Lerp(charData.primaryThemeColor, Color.white, 0.3f);
                gamchaRenderer.material = gm;
            }

            if (characterVisualHolder != null)
            {
                Renderer[] allRenderers = characterVisualHolder.GetComponentsInChildren<Renderer>();
                foreach (var r in allRenderers)
                {
                    if (r.name.ToLower().Contains("body") || r.name.ToLower().Contains("shirt") || r.name.ToLower().Contains("cloth"))
                    {
                        Material m = new Material(r.sharedMaterial);
                        m.color = charData.primaryThemeColor;
                        r.material = m;
                    }
                }
            }
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

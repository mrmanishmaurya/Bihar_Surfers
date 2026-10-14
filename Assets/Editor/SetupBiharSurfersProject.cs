using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BiharSurfers.Editor
{
    public class SetupBiharSurfersProject
    {
        [MenuItem("Bihar Surfers/Setup Complete Game Scene & Build Settings")]
        public static void SetupProject()
        {
            EnsureAssetDirectories();
            CreateGameScenes();
            ConfigureBuildSettings();
            Debug.Log("[Bihar Surfers] Project repair and Game.unity scene setup completed successfully!");
        }

        [InitializeOnLoadMethod]
        private static void OnProjectLoaded()
        {
            EditorApplication.delayCall += () =>
            {
                EnsureAssetDirectories();
                if (!File.Exists("Assets/Scenes/Game.unity"))
                {
                    SetupProject();
                }
            };
        }

        public static void EnsureAssetDirectories()
        {
            string[] folders = new string[]
            {
                "Assets/Scenes",
                "Assets/Scripts",
                "Assets/Scripts/UI",
                "Assets/Prefabs",
                "Assets/Models",
                "Assets/Materials",
                "Assets/Animations",
                "Assets/Audio",
                "Assets/UI",
                "Assets/Resources"
            };

            foreach (string f in folders)
            {
                if (!AssetDatabase.IsValidFolder(f))
                {
                    string parent = Path.GetDirectoryName(f).Replace('\\', '/');
                    string child = Path.GetFileName(f);
                    AssetDatabase.CreateFolder(parent, child);
                }
            }
        }

        public static void CreateGameScenes()
        {
            CreateScene("Assets/Scenes/Boot.unity", "Boot");
            CreateScene("Assets/Scenes/MainMenu.unity", "MainMenu");
            CreateScene("Assets/Scenes/GameOver.unity", "GameOver");
            CreateMainGameplayScene("Assets/Scenes/Game.unity");
        }

        private static void CreateScene(string scenePath, string sceneName)
        {
            if (File.Exists(scenePath)) return;

            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Add basic camera and light
            GameObject camObj = new GameObject("Main Camera");
            UnityEngine.Camera cam = camObj.AddComponent<UnityEngine.Camera>();
            camObj.tag = "MainCamera";
            camObj.transform.position = new Vector3(0, 3, -8);
            camObj.transform.rotation = Quaternion.Euler(15, 0, 0);

            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        private static void CreateMainGameplayScene(string scenePath)
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // 2. Camera
            GameObject camObj = new GameObject("Main Camera");
            UnityEngine.Camera cam = camObj.AddComponent<UnityEngine.Camera>();
            camObj.tag = "MainCamera";
            camObj.transform.position = new Vector3(0, 3.5f, -6f);
            camObj.transform.rotation = Quaternion.Euler(18f, 0, 0);
            var runnerCam = camObj.AddComponent<BiharSurfers.Camera.RunnerCamera>();

            // 3. Player GameObject
            GameObject playerObj = new GameObject("Player");
            playerObj.tag = "Player";
            playerObj.transform.position = Vector3.zero;

            var collider = playerObj.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 0.9f, 0);
            collider.height = 1.8f;
            collider.radius = 0.4f;

            var rb = playerObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var playerMovement = playerObj.AddComponent<Player.PlayerMovement>();
            var playerInput = playerObj.AddComponent<Player.PlayerInput>();
            var playerAnim = playerObj.AddComponent<Player.PlayerAnimationController>();
            var playerCollision = playerObj.AddComponent<Player.PlayerCollision>();
            var playerController = playerObj.AddComponent<Player.PlayerController>();

            playerController.movement = playerMovement;
            playerController.input = playerInput;
            playerController.animationController = playerAnim;
            playerController.collision = playerCollision;

            runnerCam.target = playerObj.transform;

            // Load Player Mesh from Assets/Models/BiharSurfersBoy/, Assets/Models/Player/ or Assets/Models/
            GameObject glbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/BiharSurfersBoy/Bihar_Surfers_Boy.glb");
            if (glbPrefab == null) glbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Player/Bihar_Surfers_Boy.glb");
            if (glbPrefab == null) glbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Bihar_Surfers_Boy.glb");

            GameObject characterVisualHolder = new GameObject("CharacterVisual");
            characterVisualHolder.transform.SetParent(playerObj.transform, false);
            characterVisualHolder.transform.localPosition = Vector3.zero;
            characterVisualHolder.transform.localRotation = Quaternion.identity;
            characterVisualHolder.transform.localScale = Vector3.one;

            if (glbPrefab != null)
            {
                GameObject characterInstance = (GameObject)PrefabUtility.InstantiatePrefab(glbPrefab, characterVisualHolder.transform);
                characterInstance.name = "Bihar_Surfers_Boy";
                characterInstance.transform.localPosition = Vector3.zero;
                characterInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                characterInstance.transform.localScale = Vector3.one;
                playerAnim.animator = characterInstance.GetComponentInChildren<Animator>();
            }
            else
            {
                // Fallback character visual primitive with Bihari Gamcha Red/Blue Kurta material
                GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "CharacterVisualPrimitive";
                body.transform.SetParent(playerObj.transform);
                body.transform.localPosition = new Vector3(0, 0.9f, 0);
                body.transform.localScale = new Vector3(0.8f, 0.9f, 0.8f);
                Environment.MaterialFactory.ApplyMaterial(body, Environment.BiharMaterialType.PlayerBody);
                Object.DestroyImmediate(body.GetComponent<Collider>());
            }

            // 4. Game Managers
            GameObject managersObj = new GameObject("GameManagers");
            managersObj.AddComponent<Core.GameManager>();
            managersObj.AddComponent<Core.ScoreManager>();
            managersObj.AddComponent<Core.DifficultyManager>();
            managersObj.AddComponent<Core.AudioManager>();
            managersObj.AddComponent<Core.SaveManager>();
            managersObj.AddComponent<Core.MissionManager>();
            managersObj.AddComponent<Core.AchievementManager>();
            managersObj.AddComponent<Environment.LocationManager>();

            // Spawners & ObjectPool
            managersObj.AddComponent<Track.ObjectPool>();
            managersObj.AddComponent<Track.TrackSpawner>();
            var trackMgr = managersObj.AddComponent<Track.TrackManager>();
            trackMgr.playerTransform = playerObj.transform;

            managersObj.AddComponent<Obstacles.ObstacleSpawner>();
            managersObj.AddComponent<Coins.CoinSpawner>();
            managersObj.AddComponent<Coins.CoinManager>();

            // 5. UI Canvas
            GameObject canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            var uiMgr = canvasObj.AddComponent<UI.UIManager>();

            // HUD Panel
            GameObject hudPanel = new GameObject("HUDPanel");
            hudPanel.transform.SetParent(canvasObj.transform, false);
            uiMgr.hudPanel = hudPanel;

            // Score Text
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(hudPanel.transform, false);
            var scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = "0";
            scoreText.fontSize = 36;
            scoreText.alignment = TextAlignmentOptions.TopLeft;
            scoreText.rectTransform.anchoredPosition = new Vector2(-150, -40);
            uiMgr.scoreText = scoreText;

            // Coins Text
            GameObject coinsObj = new GameObject("CoinsText");
            coinsObj.transform.SetParent(hudPanel.transform, false);
            var coinsText = coinsObj.AddComponent<TextMeshProUGUI>();
            coinsText.text = "0";
            coinsText.fontSize = 36;
            coinsText.alignment = TextAlignmentOptions.TopRight;
            coinsText.rectTransform.anchoredPosition = new Vector2(150, -40);
            uiMgr.coinsText = coinsText;

            // Distance Text
            GameObject distObj = new GameObject("DistanceText");
            distObj.transform.SetParent(hudPanel.transform, false);
            var distText = distObj.AddComponent<TextMeshProUGUI>();
            distText.text = "0 m";
            distText.fontSize = 28;
            distText.alignment = TextAlignmentOptions.Top;
            distText.rectTransform.anchoredPosition = new Vector2(0, -40);
            uiMgr.distanceText = distText;

            // Station Text Banner
            GameObject stationObj = new GameObject("StationText");
            stationObj.transform.SetParent(hudPanel.transform, false);
            var stationText = stationObj.AddComponent<TextMeshProUGUI>();
            stationText.text = "PATNA JUNCTION (PNBE)";
            stationText.fontSize = 24;
            stationText.alignment = TextAlignmentOptions.Top;
            stationText.rectTransform.anchoredPosition = new Vector2(0, -80);
            uiMgr.stationText = stationText;

            // Event System
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            EditorSceneManager.SaveScene(scene, scenePath);
        }

        public static void ConfigureBuildSettings()
        {
            string[] scenePaths = new string[]
            {
                "Assets/Scenes/Boot.unity",
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Game.unity",
                "Assets/Scenes/GameOver.unity"
            };

            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[scenePaths.Length];
            for (int i = 0; i < scenePaths.Length; i++)
            {
                scenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = scenes;
        }
    }
}

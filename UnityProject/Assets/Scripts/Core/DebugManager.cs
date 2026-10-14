using UnityEngine;

namespace BiharSurfers.Core
{
    /// <summary>
    /// Developer debug/cheat options (Add Coins, Set Speed, Teleport, Invincibility, Reset Save).
    /// Disabled in production release builds.
    /// </summary>
    public class DebugManager : MonoBehaviour
    {
        public static DebugManager Instance { get; private set; }

        public bool enableDebugUI = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
            enableDebugUI = false;
#endif
        }

        private void OnGUI()
        {
            if (!enableDebugUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 200, 300), "DEVELOPER DEBUG MENU", GUI.skin.window);

            if (GUILayout.Button("Add 1,000 Coins"))
            {
                SaveManager.Instance?.AddCoins(1000);
            }

            if (GUILayout.Button("Toggle Invincibility"))
            {
                if (Player.PlayerController.Instance != null)
                {
                    Player.PlayerController.Instance.isInvincible = !Player.PlayerController.Instance.isInvincible;
                }
            }

            if (GUILayout.Button("Set Max Speed"))
            {
                if (Player.PlayerController.Instance != null && Player.PlayerController.Instance.movement != null)
                {
                    Player.PlayerController.Instance.movement.forwardSpeed = 35f;
                }
            }

            if (GUILayout.Button("Reset Save Data"))
            {
                PlayerPrefs.DeleteAll();
                SaveManager.Instance?.LoadGame();
            }

            GUILayout.EndArea();
        }
    }
}

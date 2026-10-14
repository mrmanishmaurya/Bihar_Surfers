using UnityEngine;

namespace BiharSurfers.Core
{
    public enum GameState { Boot, MainMenu, Playing, Paused, GameOver }

    /// <summary>
    /// Master State Machine controlling transitions between Boot, MainMenu, Playing, Paused, and Game Over.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState currentState = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
            SetState(GameState.Playing);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }
        }

        public void SetState(GameState newState)
        {
            currentState = newState;

            switch (newState)
            {
                case GameState.Boot:
                case GameState.MainMenu:
                    Time.timeScale = 1.0f;
                    UI.UIManager.Instance?.ShowMainMenu();
                    AudioManager.Instance?.PlayMusic("MainMenuTheme");
                    break;

                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    Player.PlayerController.Instance?.ResetPlayerState();
                    Track.TrackManager.Instance?.InitTrack();
                    Coins.CoinManager.Instance?.ResetSession();
                    ScoreManager.Instance?.ResetScore();
                    DifficultyManager.Instance?.ResetDifficulty();
                    UI.UIManager.Instance?.ShowHUD();
                    AudioManager.Instance?.PlayMusic("RunningTheme");
                    break;

                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    UI.UIManager.Instance?.ShowPauseMenu();
                    AudioManager.Instance?.PauseMusic();
                    break;

                case GameState.GameOver:
                    Time.timeScale = 1.0f;
                    ScoreManager.Instance?.SaveHighScore();
                    SaveManager.Instance?.SaveGame();
                    UI.UIManager.Instance?.ShowGameOverMenu();
                    AudioManager.Instance?.PlayMusic("GameOverTheme");
                    break;
            }
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
        }

        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        public void TriggerGameOver()
        {
            SetState(GameState.GameOver);
        }

        public void RestartGame()
        {
            SetState(GameState.Playing);
        }

        public void ReturnToMainMenu()
        {
            SetState(GameState.MainMenu);
        }

        // OnGUI fallback HUD overlay for instant testing in Unity Editor / Standalone builds
        private void OnGUI()
        {
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontSize = 22;
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = new Color(1f, 0.85f, 0.2f);

            GUIStyle distStyle = new GUIStyle(GUI.skin.label);
            distStyle.fontSize = 18;
            distStyle.fontStyle = FontStyle.Bold;
            distStyle.normal.textColor = Color.white;

            GUIStyle btnStyle = new GUIStyle(GUI.skin.button);
            btnStyle.fontSize = 16;
            btnStyle.fontStyle = FontStyle.Bold;

            float distMeters = ScoreManager.Instance != null ? ScoreManager.Instance.currentDistance : 0f;

            string locName = Environment.LocationManager.Instance != null && Environment.LocationManager.Instance.GetCurrentLocation() != null ?
                Environment.LocationManager.Instance.GetCurrentLocation().stationName : "Patna Junction";

            // 1. Top HUD Panel: Title, Distance & Location
            GUI.Box(new Rect(10, 10, 270, 115), "");
            GUI.Label(new Rect(20, 15, 250, 30), "BIHAR SURFERS 3D TEST", headerStyle);
            GUI.Label(new Rect(20, 48, 250, 25), $"Distance: {Mathf.FloorToInt(distMeters)} m", distStyle);

            GUIStyle locStyle = new GUIStyle(GUI.skin.label);
            locStyle.fontSize = 15;
            locStyle.fontStyle = FontStyle.Bold;
            locStyle.normal.textColor = new Color(0.4f, 0.9f, 1.0f);
            GUI.Label(new Rect(20, 75, 250, 25), $"📍 {locName}", locStyle);

            // Pause Button (Top Right)
            if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 45), currentState == GameState.Paused ? "▶ RESUME" : "⏸ PAUSE", btnStyle))
            {
                TogglePause();
            }

            // 2. Pause Menu Overlay
            if (currentState == GameState.Paused)
            {
                GUI.Box(new Rect(Screen.width / 2 - 140, Screen.height / 2 - 110, 280, 220), "");
                GUI.Label(new Rect(Screen.width / 2 - 110, Screen.height / 2 - 90, 220, 35), "GAME PAUSED", headerStyle);

                if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 35, 180, 45), "▶ RESUME RUN", btnStyle))
                {
                    TogglePause();
                }

                if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2 + 25, 180, 45), "🔄 RESTART", btnStyle))
                {
                    RestartGame();
                }
            }

            // 3. Game Over Overlay
            if (currentState == GameState.GameOver)
            {
                GUI.Box(new Rect(Screen.width / 2 - 160, Screen.height / 2 - 170, 320, 340), "");

                GUIStyle gameOverTitleStyle = new GUIStyle(headerStyle);
                gameOverTitleStyle.normal.textColor = new Color(1f, 0.25f, 0.35f);
                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 150, 260, 35), "GAME OVER", gameOverTitleStyle);

                int finalScore = ScoreManager.Instance != null ? Mathf.FloorToInt(ScoreManager.Instance.currentScore) : 0;
                int bestScore = ScoreManager.Instance != null ? ScoreManager.Instance.highScore : 0;
                int sessionCoins = Coins.CoinManager.Instance != null ? Coins.CoinManager.Instance.sessionCoins : 0;

                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 105, 260, 25), $"Score: {finalScore}", distStyle);
                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 75, 260, 25), $"Best Score: {bestScore}", distStyle);
                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 45, 260, 25), $"Coins: +{sessionCoins}", distStyle);
                GUI.Label(new Rect(Screen.width / 2 - 130, Screen.height / 2 - 15, 260, 25), $"Distance: {Mathf.FloorToInt(distMeters)} m", distStyle);

                if (GUI.Button(new Rect(Screen.width / 2 - 110, Screen.height / 2 + 25, 220, 44), "🔄 RETRY", btnStyle))
                {
                    RestartGame();
                }

                if (GUI.Button(new Rect(Screen.width / 2 - 110, Screen.height / 2 + 80, 220, 44), "🏠 HOME", btnStyle))
                {
                    ReturnToMainMenu();
                }
            }
        }
    }
}

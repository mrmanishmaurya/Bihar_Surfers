using UnityEngine;

namespace BiharSurfers.Core
{
    public enum GameState { Boot, MainMenu, Playing, Paused, GameOver }

    /// <summary>
    /// Master State Machine controlling transitions between Boot, MainMenu, Playing, Paused, and GameOver.
    /// Coordinates audio, player state, track spawning, score calculation, and UIManager.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState currentState = GameState.MainMenu;

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
            SetState(GameState.MainMenu);
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
                    Environment.LocationManager.Instance?.ResetLocationProgression();
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
    }
}

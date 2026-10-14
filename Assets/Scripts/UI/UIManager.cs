using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BiharSurfers.UI
{
    /// <summary>
    /// Master UI Manager handling Main Menu, HUD, Pause Menu, Game Over Screen,
    /// Characters, Locations, Missions, Shop, Settings, and Daily Rewards screens.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Screen Panels")]
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject pausePanel;
        public GameObject gameOverPanel;
        public GameObject locationsPanel;
        public GameObject missionsPanel;

        [Header("Main Menu Titles")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI subtitleText;

        [Header("HUD Text Elements")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI coinsText;
        public TextMeshProUGUI distanceText;
        public TextMeshProUGUI stationText;

        [Header("Game Over Text Elements")]
        public TextMeshProUGUI finalScoreText;
        public TextMeshProUGUI bestScoreText;
        public TextMeshProUGUI finalCoinsText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.currentState == Core.GameState.Playing)
            {
                UpdateHUD();
            }
        }

        private void UpdateHUD()
        {
            if (scoreText != null && Core.ScoreManager.Instance != null)
                scoreText.text = Mathf.FloorToInt(Core.ScoreManager.Instance.currentScore).ToString();

            if (distanceText != null && Core.ScoreManager.Instance != null)
                distanceText.text = $"{Mathf.FloorToInt(Core.ScoreManager.Instance.currentDistance)} m";

            if (coinsText != null && Coins.CoinManager.Instance != null)
                coinsText.text = Coins.CoinManager.Instance.sessionCoins.ToString();

            if (stationText != null && Environment.LocationManager.Instance != null)
            {
                var loc = Environment.LocationManager.Instance.GetCurrentLocation();
                if (loc != null)
                {
                    stationText.text = $"{loc.englishName} ({loc.stationCode})";
                }
            }
        }

        public void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (locationsPanel != null) locationsPanel.SetActive(false);
            if (missionsPanel != null) missionsPanel.SetActive(false);
        }

        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

            if (titleText != null) titleText.text = "BIHAR SURFERS 3D TEST";
            if (subtitleText != null) subtitleText.text = "RUN ACROSS BIHAR";
        }

        public void ShowHUD()
        {
            HideAllPanels();
            if (hudPanel != null) hudPanel.SetActive(true);
        }

        public void ShowPauseMenu()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
        }

        public void ShowGameOverMenu()
        {
            HideAllPanels();
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);

                if (finalScoreText != null && Core.ScoreManager.Instance != null)
                    finalScoreText.text = $"{Mathf.FloorToInt(Core.ScoreManager.Instance.currentScore)} m";

                if (bestScoreText != null && Core.ScoreManager.Instance != null)
                    bestScoreText.text = $"BEST: {Core.ScoreManager.Instance.highScore} m";

                if (finalCoinsText != null && Coins.CoinManager.Instance != null)
                    finalCoinsText.text = $"+{Coins.CoinManager.Instance.sessionCoins} COINS";
            }
        }

        // --- Main Menu Navigation Button Handlers ---
        public void OnClickPlay()
        {
            Core.GameManager.Instance?.StartGame();
        }

        public void OnClickCharacters()
        {
            CharacterSelectionUI.Instance?.OpenCharacterSelection();
        }

        public void OnClickLocations()
        {
            if (locationsPanel != null) locationsPanel.SetActive(true);
        }

        public void OnClickMissions()
        {
            if (missionsPanel != null) missionsPanel.SetActive(true);
        }

        public void OnClickShop()
        {
            Shop.ShopUI.Instance?.OpenShop();
        }

        public void OnClickSettings()
        {
            SettingsUI.Instance?.OpenSettings();
        }

        public void OnClickClaimDailyReward()
        {
            if (Core.DailyRewardManager.Instance != null)
            {
                Core.DailyRewardManager.Instance.ClaimDailyReward();
            }
        }

        // --- Pause & Gameplay Button Handlers ---
        public void OnClickPause()
        {
            Core.GameManager.Instance?.TogglePause();
        }

        public void OnClickResume()
        {
            Core.GameManager.Instance?.TogglePause();
        }

        public void OnClickRestart()
        {
            Core.GameManager.Instance?.RestartGame();
        }

        public void OnClickHome()
        {
            Core.GameManager.Instance?.ReturnToMainMenu();
        }
    }
}

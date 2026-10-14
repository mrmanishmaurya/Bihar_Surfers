using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BiharSurfers.UI
{
    /// <summary>
    /// Master UI Manager handling all screens for Bihar Surfers:
    /// - Splash / Title Branding (Manish Kumar credits)
    /// - Main Menu (Play, Characters, Districts, Settings, Stats)
    /// - HUD (Score, Distance, Coins, District Milestone Banner, Pause Button)
    /// - Pause Menu (Resume, Restart, Main Menu)
    /// - Game Over Screen (Score, Best, Coins, Play Again, Main Menu)
    /// - 38-District Progression & Selection Screen
    /// - 5-Character Unlock & Selection Screen
    /// - Settings Screen (Music, SFX, Vibration)
    ///
    /// Dynamically constructs and guarantees all panels exist if not present in the scene hierarchy.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Canvas & Root")]
        public Canvas mainCanvas;

        [Header("UI Screen Panels")]
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject pausePanel;
        public GameObject gameOverPanel;
        public GameObject districtsPanel;
        public GameObject charactersPanel;
        public GameObject settingsPanel;

        [Header("HUD Text Elements")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI coinsText;
        public TextMeshProUGUI distanceText;
        public TextMeshProUGUI stationText;
        public TextMeshProUGUI stationHindiText;

        [Header("Game Over Text Elements")]
        public TextMeshProUGUI finalScoreText;
        public TextMeshProUGUI bestScoreText;
        public TextMeshProUGUI finalCoinsText;

        [Header("Main Menu Text Elements")]
        public TextMeshProUGUI menuHighScoreText;
        public TextMeshProUGUI menuCoinsText;

        // District Selection UI state
        private Transform districtListContent;
        private List<GameObject> districtCards = new List<GameObject>();

        // Character Selection UI state
        private int previewCharIndex = 0;
        private TextMeshProUGUI charNameText;
        private TextMeshProUGUI charDescText;
        private TextMeshProUGUI charCostText;
        private Image charPreviewBox;
        private Button charActionButton;
        private TextMeshProUGUI charActionBtnText;
        private TextMeshProUGUI charCoinBalanceText;

        // Settings UI text
        private TextMeshProUGUI musicStatusText;
        private TextMeshProUGUI sfxStatusText;
        private TextMeshProUGUI vibStatusText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            EnsureCanvasAndPanels();
        }

        private void Start()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.currentState == Core.GameState.MainMenu)
            {
                ShowMainMenu();
            }
            else
            {
                ShowHUD();
            }
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.currentState == Core.GameState.Playing)
            {
                UpdateHUD();
            }
        }

        private void EnsureCanvasAndPanels()
        {
            if (mainCanvas == null)
            {
                mainCanvas = GetComponentInParent<Canvas>();
            }
            if (mainCanvas == null)
            {
                GameObject canvasObj = new GameObject("UICanvas");
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            BuildMainMenuPanel();
            BuildHUDPanel();
            BuildPausePanel();
            BuildGameOverPanel();
            BuildDistrictsPanel();
            BuildCharactersPanel();
            BuildSettingsPanel();
        }

        // ==========================================
        // 1. MAIN MENU
        // ==========================================
        private void BuildMainMenuPanel()
        {
            if (mainMenuPanel != null) return;

            mainMenuPanel = CreateUIPanel(mainCanvas.transform, "MainMenuPanel", new Color(0.08f, 0.08f, 0.12f, 0.94f));

            CreateText(mainMenuPanel.transform, "TitleText", "BIHAR SURFERS 3D", 58, FontStyles.Bold, new Color(1f, 0.65f, 0.1f), new Vector2(0, 520), new Vector2(900, 140));
            CreateText(mainMenuPanel.transform, "SubtitleText", "RUN ACROSS ALL 38 DISTRICTS", 26, FontStyles.Normal, Color.white, new Vector2(0, 440), new Vector2(900, 70));
            CreateText(mainMenuPanel.transform, "CreditsText", "Created by Manish Kumar", 22, FontStyles.Italic, new Color(0.7f, 0.85f, 1f), new Vector2(0, 390), new Vector2(800, 60));

            var statsBox = CreateUIPanel(mainMenuPanel.transform, "StatsBox", new Color(0.15f, 0.15f, 0.22f, 0.85f), new Vector2(0, 260), new Vector2(800, 130));
            menuHighScoreText = CreateText(statsBox.transform, "MenuHighScore", "BEST: 0 m", 32, FontStyles.Bold, new Color(1f, 0.85f, 0.2f), new Vector2(-180, 0), new Vector2(380, 80));
            menuCoinsText = CreateText(statsBox.transform, "MenuCoins", "COINS: 0", 32, FontStyles.Bold, new Color(1f, 0.85f, 0.2f), new Vector2(180, 0), new Vector2(380, 80));

            CreateButton(mainMenuPanel.transform, "PlayButton", "RUN NOW", new Color(0.12f, 0.78f, 0.35f), new Vector2(0, 80), new Vector2(500, 110), OnClickPlay);
            CreateButton(mainMenuPanel.transform, "CharactersButton", "RUNNERS & SKINS", new Color(0.92f, 0.45f, 0.12f), new Vector2(0, -60), new Vector2(500, 95), ShowCharactersPanel);
            CreateButton(mainMenuPanel.transform, "DistrictsButton", "38 DISTRICTS MAP", new Color(0.18f, 0.58f, 0.95f), new Vector2(0, -180), new Vector2(500, 95), ShowDistrictsPanel);
            CreateButton(mainMenuPanel.transform, "SettingsButton", "SETTINGS", new Color(0.45f, 0.48f, 0.55f), new Vector2(0, -300), new Vector2(500, 90), ShowSettingsPanel);
            CreateButton(mainMenuPanel.transform, "ExitButton", "EXIT", new Color(0.72f, 0.22f, 0.22f), new Vector2(0, -410), new Vector2(300, 75), OnClickExit);
        }

        // ==========================================
        // 2. HUD PANEL
        // ==========================================
        private void BuildHUDPanel()
        {
            if (hudPanel != null) return;

            hudPanel = CreateUIPanel(mainCanvas.transform, "HUDPanel", Color.clear);

            var banner = CreateUIPanel(hudPanel.transform, "DistrictBanner", new Color(0.08f, 0.10f, 0.16f, 0.80f), new Vector2(0, 840), new Vector2(850, 110));
            stationText = CreateText(banner.transform, "StationText", "PATNA JUNCTION (PNBE)", 30, FontStyles.Bold, new Color(1f, 0.72f, 0.15f), new Vector2(0, 16), new Vector2(800, 50));
            stationHindiText = CreateText(banner.transform, "StationHindiText", "पटना जंक्शन", 24, FontStyles.Normal, Color.white, new Vector2(0, -22), new Vector2(800, 45));

            scoreText = CreateText(hudPanel.transform, "ScoreText", "0", 48, FontStyles.Bold, Color.white, new Vector2(-360, 720), new Vector2(300, 70), TextAlignmentOptions.Left);
            distanceText = CreateText(hudPanel.transform, "DistanceText", "0 m", 28, FontStyles.Normal, new Color(0.8f, 0.9f, 1f), new Vector2(-360, 660), new Vector2(300, 50), TextAlignmentOptions.Left);
            coinsText = CreateText(hudPanel.transform, "CoinsText", "0", 42, FontStyles.Bold, new Color(1f, 0.84f, 0f), new Vector2(340, 720), new Vector2(250, 70), TextAlignmentOptions.Right);

            CreateButton(hudPanel.transform, "PauseButton", "II", new Color(0.2f, 0.2f, 0.3f, 0.85f), new Vector2(460, 840), new Vector2(80, 80), OnClickPause);
        }

        // ==========================================
        // 3. PAUSE PANEL
        // ==========================================
        private void BuildPausePanel()
        {
            if (pausePanel != null) return;

            pausePanel = CreateUIPanel(mainCanvas.transform, "PausePanel", new Color(0.05f, 0.05f, 0.08f, 0.90f));
            CreateText(pausePanel.transform, "PauseTitle", "GAME PAUSED", 52, FontStyles.Bold, Color.white, new Vector2(0, 320), new Vector2(800, 100));

            CreateButton(pausePanel.transform, "ResumeBtn", "RESUME", new Color(0.12f, 0.78f, 0.35f), new Vector2(0, 120), new Vector2(450, 100), OnClickResume);
            CreateButton(pausePanel.transform, "RestartBtn", "RESTART", new Color(0.92f, 0.55f, 0.12f), new Vector2(0, -10), new Vector2(450, 95), OnClickRestart);
            CreateButton(pausePanel.transform, "QuitMenuBtn", "MAIN MENU", new Color(0.35f, 0.40f, 0.50f), new Vector2(0, -140), new Vector2(450, 95), OnClickMainMenu);

            pausePanel.SetActive(false);
        }

        // ==========================================
        // 4. GAME OVER PANEL
        // ==========================================
        private void BuildGameOverPanel()
        {
            if (gameOverPanel != null) return;

            gameOverPanel = CreateUIPanel(mainCanvas.transform, "GameOverPanel", new Color(0.06f, 0.06f, 0.10f, 0.95f));

            CreateText(gameOverPanel.transform, "GameOverTitle", "RUN COMPLETED", 54, FontStyles.Bold, new Color(1f, 0.28f, 0.28f), new Vector2(0, 520), new Vector2(800, 110));
            CreateText(gameOverPanel.transform, "GameOverSub", "खेल समाप्त", 30, FontStyles.Normal, Color.white, new Vector2(0, 440), new Vector2(800, 60));

            var resultsBox = CreateUIPanel(gameOverPanel.transform, "ResultsBox", new Color(0.14f, 0.14f, 0.20f, 0.90f), new Vector2(0, 200), new Vector2(800, 320));
            finalScoreText = CreateText(resultsBox.transform, "FinalScore", "0 m", 46, FontStyles.Bold, Color.white, new Vector2(0, 75), new Vector2(700, 70));
            bestScoreText = CreateText(resultsBox.transform, "BestScore", "BEST: 0 m", 30, FontStyles.Normal, new Color(1f, 0.85f, 0.2f), new Vector2(0, 0), new Vector2(700, 55));
            finalCoinsText = CreateText(resultsBox.transform, "FinalCoins", "+0 COINS", 36, FontStyles.Bold, new Color(1f, 0.75f, 0f), new Vector2(0, -75), new Vector2(700, 60));

            CreateButton(gameOverPanel.transform, "PlayAgainBtn", "RUN AGAIN", new Color(0.12f, 0.78f, 0.35f), new Vector2(0, -110), new Vector2(480, 105), OnClickRestart);
            CreateButton(gameOverPanel.transform, "GameOverMenuBtn", "MAIN MENU", new Color(0.30f, 0.35f, 0.45f), new Vector2(0, -240), new Vector2(480, 95), OnClickMainMenu);

            gameOverPanel.SetActive(false);
        }

        // ==========================================
        // 5. DISTRICTS PROGRESSION & SELECTION PANEL
        // ==========================================
        private void BuildDistrictsPanel()
        {
            if (districtsPanel != null) return;

            districtsPanel = CreateUIPanel(mainCanvas.transform, "DistrictsPanel", new Color(0.07f, 0.08f, 0.12f, 0.98f));

            CreateText(districtsPanel.transform, "DistHeader", "38 DISTRICTS OF BIHAR", 42, FontStyles.Bold, new Color(1f, 0.72f, 0.15f), new Vector2(0, 850), new Vector2(850, 80));
            CreateText(districtsPanel.transform, "DistSub", "Select unlocked district to start your run", 24, FontStyles.Normal, Color.white, new Vector2(0, 790), new Vector2(850, 50));

            GameObject scrollObj = new GameObject("DistrictsScrollView");
            scrollObj.transform.SetParent(districtsPanel.transform, false);
            var scrollRect = scrollObj.AddComponent<ScrollRect>();
            var sRectTransform = scrollObj.AddComponent<RectTransform>();
            sRectTransform.anchoredPosition = new Vector2(0, -40);
            sRectTransform.sizeDelta = new Vector2(950, 1400);

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollObj.transform, false);
            var vRect = viewport.AddComponent<RectTransform>();
            vRect.sizeDelta = new Vector2(950, 1400);
            var vMask = viewport.AddComponent<Mask>();
            vMask.showMaskGraphic = false;
            var vImg = viewport.AddComponent<Image>();
            vImg.color = Color.white;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            districtListContent = content.transform;
            var cRect = content.AddComponent<RectTransform>();
            cRect.anchorMin = new Vector2(0, 1);
            cRect.anchorMax = new Vector2(1, 1);
            cRect.pivot = new Vector2(0.5f, 1);
            cRect.sizeDelta = new Vector2(0, 4800);

            var vLayout = content.AddComponent<VerticalLayoutGroup>();
            vLayout.spacing = 16;
            vLayout.padding = new RectOffset(20, 20, 20, 20);
            vLayout.childControlHeight = false;
            vLayout.childControlWidth = true;
            vLayout.childForceExpandHeight = false;
            vLayout.childForceExpandWidth = true;

            scrollRect.viewport = vRect;
            scrollRect.content = cRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            CreateButton(districtsPanel.transform, "CloseDistBtn", "CLOSE", new Color(0.75f, 0.25f, 0.25f), new Vector2(0, -850), new Vector2(400, 85), CloseDistrictsPanel);

            districtsPanel.SetActive(false);
        }

        public void PopulateDistrictsList()
        {
            if (Environment.LocationManager.Instance == null || districtListContent == null) return;

            foreach (var card in districtCards)
            {
                if (card != null) Destroy(card);
            }
            districtCards.Clear();

            var list = Environment.LocationManager.Instance.locationList;
            int maxUnlocked = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.highestDistrictUnlocked : 0;

            for (int i = 0; i < list.Count; i++)
            {
                int index = i;
                var loc = list[i];
                bool isUnlocked = index <= maxUnlocked;

                Color cardCol = isUnlocked ? new Color(0.14f, 0.20f, 0.28f, 0.90f) : new Color(0.10f, 0.10f, 0.14f, 0.70f);
                GameObject card = CreateUIPanel(districtListContent, $"DistrictCard_{i}", cardCol, Vector2.zero, new Vector2(900, 105));
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 105);

                string badge = isUnlocked ? "UNLOCKED" : $"LOCKED ({loc.unlockDistance}m)";
                Color badgeCol = isUnlocked ? new Color(0.2f, 0.85f, 0.4f) : new Color(0.85f, 0.3f, 0.3f);

                CreateText(card.transform, "Num", $"#{i + 1}", 28, FontStyles.Bold, new Color(1f, 0.72f, 0.2f), new Vector2(-380, 0), new Vector2(80, 60), TextAlignmentOptions.Center);
                CreateText(card.transform, "Name", loc.districtName, 30, FontStyles.Bold, Color.white, new Vector2(-150, 14), new Vector2(360, 45), TextAlignmentOptions.Left);
                CreateText(card.transform, "Station", $"{loc.stationName} - {loc.hindiName}", 20, FontStyles.Normal, new Color(0.7f, 0.8f, 0.9f), new Vector2(-150, -18), new Vector2(360, 35), TextAlignmentOptions.Left);
                CreateText(card.transform, "Badge", badge, 22, FontStyles.Bold, badgeCol, new Vector2(170, 0), new Vector2(240, 50), TextAlignmentOptions.Center);

                if (isUnlocked)
                {
                    CreateButton(card.transform, "SelectBtn", "RUN", new Color(0.12f, 0.75f, 0.35f), new Vector2(360, 0), new Vector2(120, 70), () =>
                    {
                        Environment.LocationManager.Instance.SelectStartingDistrict(index);
                        CloseDistrictsPanel();
                        OnClickPlay();
                    });
                }

                districtCards.Add(card);
            }
        }

        // ==========================================
        // 6. CHARACTERS UNLOCK & SELECTION PANEL
        // ==========================================
        private void BuildCharactersPanel()
        {
            if (charactersPanel != null) return;

            charactersPanel = CreateUIPanel(mainCanvas.transform, "CharactersPanel", new Color(0.07f, 0.08f, 0.12f, 0.98f));

            CreateText(charactersPanel.transform, "CharHeader", "BIHAR SURFERS RUNNERS", 46, FontStyles.Bold, new Color(1f, 0.72f, 0.15f), new Vector2(0, 750), new Vector2(850, 80));

            charCoinBalanceText = CreateText(charactersPanel.transform, "CharCoinBalance", "COINS: 0", 30, FontStyles.Bold, new Color(1f, 0.85f, 0.2f), new Vector2(0, 670), new Vector2(600, 50));

            var cardBox = CreateUIPanel(charactersPanel.transform, "CharCard", new Color(0.14f, 0.16f, 0.24f, 0.90f), new Vector2(0, 160), new Vector2(820, 800));

            var pBox = CreateUIPanel(cardBox.transform, "ColorPreview", Color.orange, new Vector2(0, 220), new Vector2(320, 280));
            charPreviewBox = pBox.GetComponent<Image>();

            charNameText = CreateText(cardBox.transform, "CharName", "Aman", 44, FontStyles.Bold, Color.white, new Vector2(0, 30), new Vector2(750, 60));
            charDescText = CreateText(cardBox.transform, "CharDesc", "Agile runner from Patna.", 26, FontStyles.Normal, new Color(0.85f, 0.9f, 0.95f), new Vector2(0, -60), new Vector2(720, 110));
            charCostText = CreateText(cardBox.transform, "CharCost", "FREE", 32, FontStyles.Bold, new Color(0.2f, 0.85f, 0.4f), new Vector2(0, -150), new Vector2(600, 50));

            CreateButton(cardBox.transform, "PrevCharBtn", "<", new Color(0.25f, 0.3f, 0.45f), new Vector2(-330, 220), new Vector2(90, 110), OnPrevCharacter);
            CreateButton(cardBox.transform, "NextCharBtn", ">", new Color(0.25f, 0.3f, 0.45f), new Vector2(330, 220), new Vector2(90, 110), OnNextCharacter);

            var actBtn = CreateButton(cardBox.transform, "ActionBtn", "SELECT", new Color(0.12f, 0.78f, 0.35f), new Vector2(0, -260), new Vector2(460, 95), OnClickCharacterAction);
            charActionButton = actBtn.GetComponent<Button>();
            charActionBtnText = actBtn.GetComponentInChildren<TextMeshProUGUI>();

            CreateButton(charactersPanel.transform, "CloseCharBtn", "BACK TO MENU", new Color(0.35f, 0.40f, 0.50f), new Vector2(0, -420), new Vector2(460, 85), CloseCharactersPanel);

            charactersPanel.SetActive(false);
        }

        private void UpdateCharacterSelectionView()
        {
            if (Player.CharacterManager.Instance == null || Player.CharacterManager.Instance.characterList.Count == 0) return;

            var list = Player.CharacterManager.Instance.characterList;
            previewCharIndex = Mathf.Clamp(previewCharIndex, 0, list.Count - 1);
            var cData = list[previewCharIndex];

            if (charNameText != null) charNameText.text = cData.characterName;
            if (charDescText != null) charDescText.text = cData.description;

            if (charPreviewBox != null) charPreviewBox.color = cData.primaryThemeColor;

            int coins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (charCoinBalanceText != null) charCoinBalanceText.text = $"COINS: {coins}";

            bool isUnlocked = Player.CharacterManager.Instance.IsCharacterUnlocked(cData.characterId);
            bool isSelected = cData.characterId == Player.CharacterManager.Instance.selectedCharacterId;

            if (isSelected)
            {
                if (charCostText != null) charCostText.text = "SELECTED RUNNER";
                if (charActionBtnText != null) charActionBtnText.text = "CURRENT RUNNER";
                if (charActionButton != null) charActionButton.interactable = false;
            }
            else if (isUnlocked)
            {
                if (charCostText != null) charCostText.text = "UNLOCKED";
                if (charActionBtnText != null) charActionBtnText.text = "SELECT";
                if (charActionButton != null) charActionButton.interactable = true;
            }
            else
            {
                if (charCostText != null) charCostText.text = $"UNLOCK: {cData.unlockPriceCoins} COINS";
                if (charActionBtnText != null) charActionBtnText.text = coins >= cData.unlockPriceCoins ? "UNLOCK NOW" : "NEED MORE COINS";
                if (charActionButton != null) charActionButton.interactable = coins >= cData.unlockPriceCoins;
            }
        }

        private void OnPrevCharacter()
        {
            if (Player.CharacterManager.Instance == null) return;
            int count = Player.CharacterManager.Instance.characterList.Count;
            previewCharIndex = (previewCharIndex - 1 + count) % count;
            UpdateCharacterSelectionView();
        }

        private void OnNextCharacter()
        {
            if (Player.CharacterManager.Instance == null) return;
            int count = Player.CharacterManager.Instance.characterList.Count;
            previewCharIndex = (previewCharIndex + 1) % count;
            UpdateCharacterSelectionView();
        }

        private void OnClickCharacterAction()
        {
            if (Player.CharacterManager.Instance == null) return;
            var cData = Player.CharacterManager.Instance.characterList[previewCharIndex];

            if (Player.CharacterManager.Instance.IsCharacterUnlocked(cData.characterId))
            {
                Player.CharacterManager.Instance.SelectCharacter(cData.characterId);
            }
            else
            {
                Player.CharacterManager.Instance.UnlockCharacter(cData.characterId);
            }
            UpdateCharacterSelectionView();
        }

        // ==========================================
        // 7. SETTINGS PANEL
        // ==========================================
        private void BuildSettingsPanel()
        {
            if (settingsPanel != null) return;

            settingsPanel = CreateUIPanel(mainCanvas.transform, "SettingsPanel", new Color(0.07f, 0.08f, 0.12f, 0.98f));

            CreateText(settingsPanel.transform, "SetHeader", "GAME SETTINGS", 46, FontStyles.Bold, new Color(1f, 0.72f, 0.15f), new Vector2(0, 480), new Vector2(850, 80));

            var mBtn = CreateButton(settingsPanel.transform, "MusicToggle", "MUSIC: ON", new Color(0.20f, 0.25f, 0.35f), new Vector2(0, 240), new Vector2(550, 95), OnToggleMusic);
            musicStatusText = mBtn.GetComponentInChildren<TextMeshProUGUI>();

            var sBtn = CreateButton(settingsPanel.transform, "SFXToggle", "SOUND FX: ON", new Color(0.20f, 0.25f, 0.35f), new Vector2(0, 110), new Vector2(550, 95), OnToggleSFX);
            sfxStatusText = sBtn.GetComponentInChildren<TextMeshProUGUI>();

            var vBtn = CreateButton(settingsPanel.transform, "VibToggle", "VIBRATION: ON", new Color(0.20f, 0.25f, 0.35f), new Vector2(0, -20), new Vector2(550, 95), OnToggleVibration);
            vibStatusText = vBtn.GetComponentInChildren<TextMeshProUGUI>();

            CreateButton(settingsPanel.transform, "CloseSetBtn", "CLOSE", new Color(0.72f, 0.25f, 0.25f), new Vector2(0, -220), new Vector2(400, 85), CloseSettingsPanel);

            settingsPanel.SetActive(false);
        }

        private void UpdateSettingsView()
        {
            bool musicOn = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.isMusicOn : true;
            bool sfxOn = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.isSFXOn : true;
            bool vibOn = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.isVibrationOn : true;

            if (musicStatusText != null) musicStatusText.text = musicOn ? "MUSIC: ON" : "MUSIC: OFF";
            if (sfxStatusText != null) sfxStatusText.text = sfxOn ? "SOUND FX: ON" : "SOUND FX: OFF";
            if (vibStatusText != null) vibStatusText.text = vibOn ? "VIBRATION: ON" : "VIBRATION: OFF";
        }

        private void OnToggleMusic()
        {
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.isMusicOn = !Core.SaveManager.Instance.data.isMusicOn;
                Core.SaveManager.Instance.SaveGame();
            }
            UpdateSettingsView();
        }

        private void OnToggleSFX()
        {
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.isSFXOn = !Core.SaveManager.Instance.data.isSFXOn;
                Core.SaveManager.Instance.SaveGame();
            }
            UpdateSettingsView();
        }

        private void OnToggleVibration()
        {
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.isVibrationOn = !Core.SaveManager.Instance.data.isVibrationOn;
                Core.SaveManager.Instance.SaveGame();
            }
            UpdateSettingsView();
        }

        // ==========================================
        // SCREEN VISIBILITY CONTROLS
        // ==========================================
        public void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (districtsPanel != null) districtsPanel.SetActive(false);
            if (charactersPanel != null) charactersPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ShowMainMenu()
        {
            HideAllPanels();
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);

                int highScore = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.highScore : 0;
                int coins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;

                if (menuHighScoreText != null) menuHighScoreText.text = $"BEST: {highScore} m";
                if (menuCoinsText != null) menuCoinsText.text = $"COINS: {coins}";
            }
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

                float score = Core.ScoreManager.Instance != null ? Core.ScoreManager.Instance.currentScore : 0f;
                int highScore = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.highScore : 0;
                int sessionCoins = Coins.CoinManager.Instance != null ? Coins.CoinManager.Instance.sessionCoins : 0;

                if (finalScoreText != null) finalScoreText.text = $"{Mathf.FloorToInt(score)} m";
                if (bestScoreText != null) bestScoreText.text = $"BEST: {highScore} m";
                if (finalCoinsText != null) finalCoinsText.text = $"+{sessionCoins} COINS";
            }
        }

        public void ShowDistrictsPanel()
        {
            HideAllPanels();
            if (districtsPanel != null)
            {
                districtsPanel.SetActive(true);
                PopulateDistrictsList();
            }
        }

        public void CloseDistrictsPanel()
        {
            ShowMainMenu();
        }

        public void ShowCharactersPanel()
        {
            HideAllPanels();
            if (charactersPanel != null)
            {
                charactersPanel.SetActive(true);
                UpdateCharacterSelectionView();
            }
        }

        public void CloseCharactersPanel()
        {
            ShowMainMenu();
        }

        public void ShowSettingsPanel()
        {
            HideAllPanels();
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
                UpdateSettingsView();
            }
        }

        public void CloseSettingsPanel()
        {
            ShowMainMenu();
        }

        // ==========================================
        // HUD UPDATE
        // ==========================================
        private void UpdateHUD()
        {
            if (scoreText != null && Core.ScoreManager.Instance != null)
                scoreText.text = Mathf.FloorToInt(Core.ScoreManager.Instance.currentScore).ToString();

            if (distanceText != null && Core.ScoreManager.Instance != null)
                distanceText.text = $"{Mathf.FloorToInt(Core.ScoreManager.Instance.currentDistance)} m";

            if (coinsText != null && Coins.CoinManager.Instance != null)
                coinsText.text = Coins.CoinManager.Instance.sessionCoins.ToString();

            if (Environment.LocationManager.Instance != null)
            {
                var loc = Environment.LocationManager.Instance.GetCurrentLocation();
                if (loc != null)
                {
                    if (stationText != null) stationText.text = $"{loc.englishName} ({loc.stationCode})";
                    if (stationHindiText != null) stationHindiText.text = loc.hindiName;
                }
            }
        }

        // ==========================================
        // BUTTON HANDLERS
        // ==========================================
        public void OnClickPlay()
        {
            Core.GameManager.Instance?.StartGame();
        }

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

        public void OnClickMainMenu()
        {
            Core.GameManager.Instance?.SetState(Core.GameState.MainMenu);
        }

        public void OnClickExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ==========================================
        // UI ELEMENT HELPERS
        // ==========================================
        private GameObject CreateUIPanel(Transform parent, string name, Color color, Vector2 pos = default, Vector2 size = default)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();

            if (size == default)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.sizeDelta = Vector2.zero;
            }
            else
            {
                rect.anchoredPosition = pos;
                rect.sizeDelta = size;
            }

            var img = obj.AddComponent<Image>();
            img.color = color;
            return obj;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, FontStyles style, Color color, Vector2 pos, Vector2 size, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.color = color;
            tmp.alignment = align;
            return tmp;
        }

        private GameObject CreateButton(Transform parent, string name, string label, Color bgColor, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction onClick)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            var rect = btnObj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            var img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            var btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            var tRect = textObj.AddComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = Vector2.zero;

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 28;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return btnObj;
        }
    }
}

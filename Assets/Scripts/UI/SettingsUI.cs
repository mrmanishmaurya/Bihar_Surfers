using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BiharSurfers.UI
{
    /// <summary>
    /// Settings screen managing Music toggle, SFX toggle, Vibration toggle,
    /// Graphics Quality preset (Low/Medium/High), and English/Hindi language switching.
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance { get; private set; }

        [Header("Settings Panel")]
        public GameObject settingsPanel;

        [Header("Toggle Buttons & Displays")]
        public TextMeshProUGUI musicText;
        public TextMeshProUGUI sfxText;
        public TextMeshProUGUI vibrationText;
        public TextMeshProUGUI qualityText;
        public TextMeshProUGUI languageText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
            UpdateUI();
        }

        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ToggleMusic()
        {
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.isMusicOn = !Core.SaveManager.Instance.data.isMusicOn;
                Core.SaveManager.Instance.SaveGame();
            }
            UpdateUI();
        }

        public void ToggleSFX()
        {
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.isSFXOn = !Core.SaveManager.Instance.data.isSFXOn;
                Core.SaveManager.Instance.SaveGame();
            }
            UpdateUI();
        }

        public void ToggleVibration()
        {
            bool current = PlayerPrefs.GetInt("BiharSurfers_Vib", 1) == 1;
            PlayerPrefs.SetInt("BiharSurfers_Vib", current ? 0 : 1);
            PlayerPrefs.Save();
            UpdateUI();
        }

        public void CycleGraphicsQuality()
        {
            int currentLevel = QualitySettings.GetQualityLevel();
            int nextLevel = (currentLevel + 1) % 3;
            QualitySettings.SetQualityLevel(nextLevel, true);
            UpdateUI();
        }

        public void ToggleLanguage()
        {
            if (Core.LocalizationManager.Instance != null)
            {
                var nextLang = Core.LocalizationManager.Instance.currentLanguage == Core.Language.English ?
                    Core.Language.Hindi : Core.Language.English;
                Core.LocalizationManager.Instance.SetLanguage(nextLang);
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            bool musicOn = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.isMusicOn : true;
            bool sfxOn = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.isSFXOn : true;
            bool vibOn = PlayerPrefs.GetInt("BiharSurfers_Vib", 1) == 1;

            if (musicText != null) musicText.text = musicOn ? "MUSIC: ON" : "MUSIC: OFF";
            if (sfxText != null) sfxText.text = sfxOn ? "SFX: ON" : "SFX: OFF";
            if (vibrationText != null) vibrationText.text = vibOn ? "VIBRATION: ON" : "VIBRATION: OFF";

            int quality = QualitySettings.GetQualityLevel();
            string qualityName = quality == 0 ? "LOW" : (quality == 1 ? "MEDIUM" : "HIGH");
            if (qualityText != null) qualityText.text = $"GRAPHICS: {qualityName}";

            string langName = Core.LocalizationManager.Instance != null ? Core.LocalizationManager.Instance.currentLanguage.ToString() : "English";
            if (languageText != null) languageText.text = $"LANGUAGE: {langName}";
        }
    }
}

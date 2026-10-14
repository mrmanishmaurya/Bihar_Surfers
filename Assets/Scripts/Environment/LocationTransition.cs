using TMPro;
using UnityEngine;

namespace BiharSurfers.Environment
{
    /// <summary>
    /// Manages location banner transitions ("WELCOME TO PATNA", "WELCOME TO ARA"),
    /// transition animations, lighting adjustments, and station chime audio.
    /// </summary>
    public class LocationTransition : MonoBehaviour
    {
        public static LocationTransition Instance { get; private set; }

        [Header("UI Canvas Overlay")]
        public GameObject transitionPanel;
        public TextMeshProUGUI hindiTitleText;
        public TextMeshProUGUI englishTitleText;
        public TextMeshProUGUI stationCodeText;
        public float bannerDisplayDuration = 3.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void TriggerLocationTransition(Data.LocationData locationData)
        {
            if (locationData == null) return;

            // Update UI Banner Text
            if (hindiTitleText != null) hindiTitleText.text = locationData.hindiName;
            if (englishTitleText != null) englishTitleText.text = $"WELCOME TO {locationData.englishName}";
            if (stationCodeText != null) stationCodeText.text = $"[{locationData.stationCode}]";

            // Activate Banner Overlay
            if (transitionPanel != null)
            {
                transitionPanel.SetActive(true);
                CancelInvoke(nameof(HideTransitionBanner));
                Invoke(nameof(HideTransitionBanner), bannerDisplayDuration);
            }

            // Apply Skybox and Fog Lighting Theme
            if (locationData.skyboxMaterial != null)
            {
                RenderSettings.skybox = locationData.skyboxMaterial;
            }
            RenderSettings.fog = true;
            RenderSettings.fogColor = locationData.fogColor;

            // Audio & Camera Shake FX
            Core.AudioManager.Instance?.PlaySFX("LocationTransition");
            Camera.RunnerCamera.Instance?.TriggerShake(0.4f, 0.25f);
        }

        private void HideTransitionBanner()
        {
            if (transitionPanel != null)
            {
                transitionPanel.SetActive(false);
            }
        }
    }
}

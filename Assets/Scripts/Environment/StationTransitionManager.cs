using TMPro;
using UnityEngine;

namespace BiharSurfers.Environment
{
    /// <summary>
    /// Displays station transition banners ("PATNA JUNCTION", "WELCOME TO ARA")
    /// with cinematic fade and animation effects.
    /// </summary>
    public class StationTransitionManager : MonoBehaviour
    {
        public static StationTransitionManager Instance { get; private set; }

        [Header("UI Canvas Elements")]
        public GameObject transitionCanvasGroup;
        public TextMeshProUGUI hindiText;
        public TextMeshProUGUI englishText;
        public float bannerDuration = 3.0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ShowTransitionBanner(string nameHi, string nameEn)
        {
            if (transitionCanvasGroup == null) return;

            if (hindiText != null) hindiText.text = nameHi;
            if (englishText != null) englishText.text = $"WELCOME TO {nameEn}";

            transitionCanvasGroup.SetActive(true);
            CancelInvoke(nameof(HideBanner));
            Invoke(nameof(HideBanner), bannerDuration);
        }

        private void HideBanner()
        {
            if (transitionCanvasGroup != null)
            {
                transitionCanvasGroup.SetActive(false);
            }
        }
    }
}

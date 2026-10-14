using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    public enum Language { English, Hindi }

    /// <summary>
    /// Basic localization system supporting English and Hindi UI text strings.
    /// Extensible for additional languages.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public Language currentLanguage = Language.English;

        private Dictionary<string, string> englishDict = new Dictionary<string, string>();
        private Dictionary<string, string> hindiDict = new Dictionary<string, string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitDictionaries();
        }

        private void InitDictionaries()
        {
            // English Dictionary
            englishDict["PLAY"] = "PLAY RUN";
            englishDict["GAME_OVER"] = "GAME OVER";
            englishDict["HIGH_SCORE"] = "HIGH SCORE";
            englishDict["RETRY"] = "RETRY";
            englishDict["SETTINGS"] = "SETTINGS";
            englishDict["LOCATIONS"] = "LOCATIONS";
            englishDict["CHARACTERS"] = "CHARACTERS";
            englishDict["MISSIONS"] = "MISSIONS";

            // Hindi Dictionary
            hindiDict["PLAY"] = "खेल शुरू करें";
            hindiDict["GAME_OVER"] = "खेल समाप्त";
            hindiDict["HIGH_SCORE"] = "सर्वश्रेष्ठ स्कोर";
            hindiDict["RETRY"] = "पुनः प्रयास करें";
            hindiDict["SETTINGS"] = "सेटिंग्स";
            hindiDict["LOCATIONS"] = "बिहार के स्थान";
            hindiDict["CHARACTERS"] = "खिलाड़ी";
            hindiDict["MISSIONS"] = "मिशन";
        }

        public string GetText(string key)
        {
            if (currentLanguage == Language.Hindi)
            {
                if (hindiDict.ContainsKey(key)) return hindiDict[key];
            }
            else
            {
                if (englishDict.ContainsKey(key)) return englishDict[key];
            }
            return key;
        }

        public void SetLanguage(Language lang)
        {
            currentLanguage = lang;
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.data.language = lang.ToString();
                SaveManager.Instance.SaveGame();
            }
        }
    }
}

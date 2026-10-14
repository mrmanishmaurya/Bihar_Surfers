using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class SaveData
    {
        public int coins = 0;
        public int highScore = 0;
        public int currentDistrictIndex = 0;
        public int highestDistrictUnlocked = 0;
        public string selectedCharacter = "Aman";
        public string selectedSkin = "Casual";
        public List<string> unlockedCharacters = new List<string> { "Aman" };
        public bool isMusicOn = true;
        public bool isSFXOn = true;
        public bool isVibrationOn = true;
        public string language = "English";
    }

    /// <summary>
    /// Handles local JSON file persistence with error handling, data corruption protection,
    /// and full support for all 38 districts and unlockable characters.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public SaveData data = new SaveData();
        private string saveFilePath;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            saveFilePath = Path.Combine(Application.persistentDataPath, "biharsurfers_save.json");
            LoadGame();
        }

        public void SaveGame()
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(saveFilePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game data: {e.Message}");
            }
        }

        public void LoadGame()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string json = File.ReadAllText(saveFilePath);
                    data = JsonUtility.FromJson<SaveData>(json);

                    if (data == null)
                    {
                        data = new SaveData();
                    }
                    if (data.unlockedCharacters == null || data.unlockedCharacters.Count == 0)
                    {
                        data.unlockedCharacters = new List<string> { "Aman" };
                    }
                }
                else
                {
                    data = new SaveData();
                    SaveGame();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Save data corrupted, resetting to defaults: {e.Message}");
                data = new SaveData();
                SaveGame();
            }
        }

        public void AddCoins(int amount)
        {
            data.coins += amount;
            SaveGame();
        }

        public bool IsCharacterUnlocked(string charId)
        {
            if (string.IsNullOrEmpty(charId)) return false;
            if (charId == "Aman") return true;
            return data.unlockedCharacters != null && data.unlockedCharacters.Contains(charId);
        }

        public void UnlockCharacter(string charId)
        {
            if (data.unlockedCharacters == null)
                data.unlockedCharacters = new List<string>();

            if (!data.unlockedCharacters.Contains(charId))
            {
                data.unlockedCharacters.Add(charId);
                SaveGame();
            }
        }

        public bool IsDistrictUnlocked(int index)
        {
            return index <= data.highestDistrictUnlocked;
        }

        public void UnlockDistrict(int index)
        {
            if (index > data.highestDistrictUnlocked)
            {
                data.highestDistrictUnlocked = index;
                SaveGame();
            }
        }
    }
}

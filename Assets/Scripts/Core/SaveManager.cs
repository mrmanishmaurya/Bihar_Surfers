using System.IO;
using UnityEngine;

namespace BiharSurfers.Core
{
    [System.Serializable]
    public class SaveData
    {
        public int coins = 0;
        public int highScore = 0;
        public int currentDistrictIndex = 0;
        public string selectedCharacter = "Aman";
        public string selectedSkin = "Casual";
        public bool isMusicOn = true;
        public bool isSFXOn = true;
        public string language = "English";
    }

    /// <summary>
    /// Handles local JSON file persistence with error handling and data corruption protection.
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
    }
}

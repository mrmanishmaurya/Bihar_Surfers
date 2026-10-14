using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Manages 5 playable characters (Aman, Ravi, Sonu, Vikash, Arjun),
    /// skin categories (Casual, Student, Sports, Traditional, Explorer),
    /// persistent coin unlocks, and active character selection.
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [Header("Character Registry")]
        public List<Data.CharacterData> characterList = new List<Data.CharacterData>();
        public string selectedCharacterId = "Aman";
        public string selectedSkinId = "Casual";

        public System.Action<Data.CharacterData> OnCharacterChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeDefaultCharacters();
            LoadSavedSelection();
        }

        private void LoadSavedSelection()
        {
            if (Core.SaveManager.Instance != null && !string.IsNullOrEmpty(Core.SaveManager.Instance.data.selectedCharacter))
            {
                selectedCharacterId = Core.SaveManager.Instance.data.selectedCharacter;
                selectedSkinId = Core.SaveManager.Instance.data.selectedSkin ?? "Casual";
            }
        }

        private void InitializeDefaultCharacters()
        {
            if (characterList.Count > 0) return;

            // 1. Aman (Default)
            var c1 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c1.characterId = "Aman";
            c1.characterName = "Aman";
            c1.description = "Agile Patna express runner. Fast footwork, quick reflexes, and signature Bihari red gamcha.";
            c1.unlockPriceCoins = 0;
            c1.isDefaultUnlocked = true;
            c1.primaryThemeColor = new Color(0.95f, 0.45f, 0.10f); // Bright Saffron
            AddDefaultSkins(c1);
            characterList.Add(c1);

            // 2. Ravi
            var c2 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c2.characterId = "Ravi";
            c2.characterName = "Ravi";
            c2.description = "Bhojpuri champion from Ara with high jump endurance and green athlete jersey.";
            c2.unlockPriceCoins = 1500;
            c2.isDefaultUnlocked = false;
            c2.primaryThemeColor = new Color(0.18f, 0.72f, 0.32f); // Emerald Green
            AddDefaultSkins(c2);
            characterList.Add(c2);

            // 3. Sonu
            var c3 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c3.characterId = "Sonu";
            c3.characterName = "Sonu";
            c3.description = "Ganga riverfront sprinter from Buxar with rapid slide recovery and ocean blue attire.";
            c3.unlockPriceCoins = 3000;
            c3.isDefaultUnlocked = false;
            c3.primaryThemeColor = new Color(0.15f, 0.65f, 0.95f); // Ganga Blue
            AddDefaultSkins(c3);
            characterList.Add(c3);

            // 4. Vikash
            var c4 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c4.characterId = "Vikash";
            c4.characterName = "Vikash";
            c4.description = "Historic fort explorer from Sasaram with extended magnet radius and crimson desert vest.";
            c4.unlockPriceCoins = 5000;
            c4.isDefaultUnlocked = false;
            c4.primaryThemeColor = new Color(0.92f, 0.22f, 0.25f); // Crimson Red
            AddDefaultSkins(c4);
            characterList.Add(c4);

            // 5. Arjun
            var c5 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c5.characterId = "Arjun";
            c5.characterName = "Arjun";
            c5.description = "Master railway navigator from Chhapra with 2x coin multiplier bonus and royal purple kurta.";
            c5.unlockPriceCoins = 8000;
            c5.isDefaultUnlocked = false;
            c5.primaryThemeColor = new Color(0.68f, 0.28f, 0.88f); // Royal Purple
            AddDefaultSkins(c5);
            characterList.Add(c5);
        }

        private void AddDefaultSkins(Data.CharacterData charData)
        {
            charData.skins = new List<Data.SkinData>
            {
                new Data.SkinData { skinId = "Casual", skinName = "Casual Runner", category = Data.SkinCategory.Casual, unlockPriceCoins = 0, isUnlocked = true },
                new Data.SkinData { skinId = "Student", skinName = "Patna Student", category = Data.SkinCategory.Student, unlockPriceCoins = 500, isUnlocked = false },
                new Data.SkinData { skinId = "Sports", skinName = "Bihari Sprinter", category = Data.SkinCategory.Sports, unlockPriceCoins = 1000, isUnlocked = false },
                new Data.SkinData { skinId = "Traditional", skinName = "Chhath Kurta", category = Data.SkinCategory.Traditional, unlockPriceCoins = 2000, isUnlocked = false },
                new Data.SkinData { skinId = "Explorer", skinName = "Railway Explorer", category = Data.SkinCategory.Explorer, unlockPriceCoins = 3500, isUnlocked = false }
            };
        }

        public bool IsCharacterUnlocked(string charId)
        {
            if (string.IsNullOrEmpty(charId)) return false;
            if (charId == "Aman") return true;

            if (Core.SaveManager.Instance != null)
            {
                return Core.SaveManager.Instance.IsCharacterUnlocked(charId);
            }
            return false;
        }

        public bool UnlockCharacter(string charId)
        {
            Data.CharacterData charData = characterList.Find(c => c.characterId == charId);
            if (charData == null) return false;

            if (IsCharacterUnlocked(charId)) return true;

            int currentCoins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (currentCoins >= charData.unlockPriceCoins)
            {
                if (Core.SaveManager.Instance != null)
                {
                    Core.SaveManager.Instance.data.coins -= charData.unlockPriceCoins;
                    Core.SaveManager.Instance.UnlockCharacter(charId);
                    Core.SaveManager.Instance.data.selectedCharacter = charId;
                    Core.SaveManager.Instance.SaveGame();
                }

                selectedCharacterId = charId;
                Core.AudioManager.Instance?.PlaySFX("PurchaseSuccess");
                OnCharacterChanged?.Invoke(charData);

                PlayerController.Instance?.ApplyCharacterVisuals(charData);
                return true;
            }

            Core.AudioManager.Instance?.PlaySFX("PurchaseFail");
            return false;
        }

        public void SelectCharacter(string charId)
        {
            if (!IsCharacterUnlocked(charId)) return;

            selectedCharacterId = charId;
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.selectedCharacter = charId;
                Core.SaveManager.Instance.SaveGame();
            }

            var charData = GetSelectedCharacter();
            OnCharacterChanged?.Invoke(charData);
            PlayerController.Instance?.ApplyCharacterVisuals(charData);
        }

        public Data.CharacterData GetSelectedCharacter()
        {
            return characterList.Find(c => c.characterId == selectedCharacterId) ?? characterList[0];
        }

        public Data.CharacterData GetCharacter(string charId)
        {
            return characterList.Find(c => c.characterId == charId);
        }
    }
}

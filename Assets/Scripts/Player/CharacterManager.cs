using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Player
{
    /// <summary>
    /// Manages character unlocks (Aman, Ravi, Sonu, Vikash, Arjun), skin categories
    /// (Casual, Student, Sports, Traditional, Explorer), active character selection, and coin purchases.
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }

        [Header("Character Registry")]
        public List<Data.CharacterData> characterList = new List<Data.CharacterData>();
        public string selectedCharacterId = "Aman";
        public string selectedSkinId = "Casual";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeDefaultCharacters();
        }

        private void InitializeDefaultCharacters()
        {
            if (characterList.Count > 0) return;

            // 1. Aman (Default)
            var c1 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c1.characterId = "Aman";
            c1.characterName = "Aman";
            c1.description = "Agile Patna express runner. Fast footwork and quick reflexes.";
            c1.unlockPriceCoins = 0;
            c1.isDefaultUnlocked = true;
            c1.primaryThemeColor = new Color(0.95f, 0.5f, 0.1f);
            AddDefaultSkins(c1);
            characterList.Add(c1);

            // 2. Ravi
            var c2 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c2.characterId = "Ravi";
            c2.characterName = "Ravi";
            c2.description = "Bhojpuri champion from Ara with high jump endurance.";
            c2.unlockPriceCoins = 1500;
            c2.isDefaultUnlocked = false;
            c2.primaryThemeColor = new Color(0.2f, 0.7f, 0.9f);
            AddDefaultSkins(c2);
            characterList.Add(c2);

            // 3. Sonu
            var c3 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c3.characterId = "Sonu";
            c3.characterName = "Sonu";
            c3.description = "Ganga riverfront sprinter from Buxar.";
            c3.unlockPriceCoins = 3000;
            c3.isDefaultUnlocked = false;
            c3.primaryThemeColor = new Color(0.3f, 0.8f, 0.4f);
            AddDefaultSkins(c3);
            characterList.Add(c3);

            // 4. Vikash
            var c4 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c4.characterId = "Vikash";
            c4.characterName = "Vikash";
            c4.description = "Historic fort explorer from Sasaram with magnet bonus.";
            c4.unlockPriceCoins = 5000;
            c4.isDefaultUnlocked = false;
            c4.primaryThemeColor = new Color(0.9f, 0.3f, 0.3f);
            AddDefaultSkins(c4);
            characterList.Add(c4);

            // 5. Arjun
            var c5 = ScriptableObject.CreateInstance<Data.CharacterData>();
            c5.characterId = "Arjun";
            c5.characterName = "Arjun";
            c5.description = "Master railway navigator from Chhapra.";
            c5.unlockPriceCoins = 8000;
            c5.isDefaultUnlocked = false;
            c5.primaryThemeColor = new Color(0.8f, 0.4f, 0.9f);
            AddDefaultSkins(c5);
            characterList.Add(c5);
        }

        private void AddDefaultSkins(Data.CharacterData charData)
        {
            charData.skins = new List<Data.SkinData>
            {
                new Data.SkinData { skinId = "Casual", skinName = "Casual Outfit", category = Data.SkinCategory.Casual, unlockPriceCoins = 0, isUnlocked = true },
                new Data.SkinData { skinId = "Student", skinName = "School Student", category = Data.SkinCategory.Student, unlockPriceCoins = 500, isUnlocked = false },
                new Data.SkinData { skinId = "Sports", skinName = "Sports Jersey", category = Data.SkinCategory.Sports, unlockPriceCoins = 1000, isUnlocked = false },
                new Data.SkinData { skinId = "Traditional", skinName = "Festival Kurta", category = Data.SkinCategory.Traditional, unlockPriceCoins = 2000, isUnlocked = false },
                new Data.SkinData { skinId = "Explorer", skinName = "Bihar Explorer", category = Data.SkinCategory.Explorer, unlockPriceCoins = 3500, isUnlocked = false }
            };
        }

        public bool UnlockCharacter(string charId)
        {
            Data.CharacterData charData = characterList.Find(c => c.characterId == charId);
            if (charData == null) return false;

            int currentCoins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (currentCoins >= charData.unlockPriceCoins)
            {
                Core.SaveManager.Instance.data.coins -= charData.unlockPriceCoins;
                selectedCharacterId = charId;
                Core.SaveManager.Instance.data.selectedCharacter = charId;
                Core.SaveManager.Instance.SaveGame();
                Core.AudioManager.Instance?.PlaySFX("PurchaseSuccess");
                return true;
            }

            Core.AudioManager.Instance?.PlaySFX("PurchaseFail");
            return false;
        }

        public bool UnlockSkin(string charId, string skinId)
        {
            Data.CharacterData charData = characterList.Find(c => c.characterId == charId);
            if (charData == null) return false;

            Data.SkinData skin = charData.skins.Find(s => s.skinId == skinId);
            if (skin == null || skin.isUnlocked) return false;

            int currentCoins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (currentCoins >= skin.unlockPriceCoins)
            {
                Core.SaveManager.Instance.data.coins -= skin.unlockPriceCoins;
                skin.isUnlocked = true;
                selectedSkinId = skinId;
                Core.SaveManager.Instance.data.selectedSkin = skinId;
                Core.SaveManager.Instance.SaveGame();
                Core.AudioManager.Instance?.PlaySFX("PurchaseSuccess");
                return true;
            }

            Core.AudioManager.Instance?.PlaySFX("PurchaseFail");
            return false;
        }

        public Data.CharacterData GetSelectedCharacter()
        {
            return characterList.Find(c => c.characterId == selectedCharacterId) ?? characterList[0];
        }
    }
}

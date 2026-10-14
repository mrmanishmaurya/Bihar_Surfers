using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Data
{
    public enum SkinCategory { Casual, Student, Sports, Traditional, Explorer }

    [System.Serializable]
    public class SkinData
    {
        public string skinId;
        public string skinName;
        public SkinCategory category = SkinCategory.Casual;
        public int unlockPriceCoins = 500;
        public Color primaryColor = Color.orange;
        public Color secondaryColor = Color.blue;
        public bool isUnlocked = false;
    }

    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "BiharSurfers/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Character Identity")]
        public string characterId = "Aman";
        public string characterName = "Aman";
        [TextArea] public string description = "A brave Bihari runner from Patna.";
        public int unlockPriceCoins = 0; // 0 = Free default
        public bool isDefaultUnlocked = true;

        [Header("Visual & Prefab Settings")]
        public GameObject characterPrefab;
        public Sprite characterIcon;
        public Color primaryThemeColor = new Color(0.95f, 0.5f, 0.1f);

        [Header("Character Skins")]
        public List<SkinData> skins = new List<SkinData>();
    }
}

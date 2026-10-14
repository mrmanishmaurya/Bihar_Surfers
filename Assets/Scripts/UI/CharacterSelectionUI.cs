using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BiharSurfers.UI
{
    /// <summary>
    /// Handles character selection UI screen, skin category previewing,
    /// coin balance updates, and character/skin unlock purchase interactions.
    /// </summary>
    public class CharacterSelectionUI : MonoBehaviour
    {
        public static CharacterSelectionUI Instance { get; private set; }

        [Header("UI Panel")]
        public GameObject characterSelectPanel;

        [Header("Character Preview Display")]
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI unlockCostText;
        public Image characterIconImage;
        public Button unlockButton;
        public Button selectButton;

        [Header("Coin Balance Header")]
        public TextMeshProUGUI coinBalanceText;

        private int activeCharIndex = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void OpenCharacterSelection()
        {
            if (characterSelectPanel != null) characterSelectPanel.SetActive(true);
            activeCharIndex = 0;
            UpdateCharacterPreview();
        }

        public void CloseCharacterSelection()
        {
            if (characterSelectPanel != null) characterSelectPanel.SetActive(false);
        }

        public void NextCharacter()
        {
            if (Player.CharacterManager.Instance == null || Player.CharacterManager.Instance.characterList.Count == 0) return;
            activeCharIndex = (activeCharIndex + 1) % Player.CharacterManager.Instance.characterList.Count;
            UpdateCharacterPreview();
        }

        public void PreviousCharacter()
        {
            if (Player.CharacterManager.Instance == null || Player.CharacterManager.Instance.characterList.Count == 0) return;
            activeCharIndex = (activeCharIndex - 1 + Player.CharacterManager.Instance.characterList.Count) % Player.CharacterManager.Instance.characterList.Count;
            UpdateCharacterPreview();
        }

        private void UpdateCharacterPreview()
        {
            if (Player.CharacterManager.Instance == null || Player.CharacterManager.Instance.characterList.Count == 0) return;

            var charData = Player.CharacterManager.Instance.characterList[activeCharIndex];

            if (characterNameText != null) characterNameText.text = charData.characterName;
            if (descriptionText != null) descriptionText.text = charData.description;
            if (unlockCostText != null) unlockCostText.text = $"{charData.unlockPriceCoins} COINS";

            int currentCoins = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.data.coins : 0;
            if (coinBalanceText != null) coinBalanceText.text = $"{currentCoins} 🪙";

            bool isSelected = charData.characterId == Player.CharacterManager.Instance.selectedCharacterId;
            bool isUnlocked = charData.isDefaultUnlocked || (Core.SaveManager.Instance != null && Core.SaveManager.Instance.data.coins >= 0);

            if (unlockButton != null) unlockButton.gameObject.SetActive(!isUnlocked);
            if (selectButton != null) selectButton.gameObject.SetActive(isUnlocked);
        }

        public void OnClickUnlock()
        {
            if (Player.CharacterManager.Instance == null) return;
            var charData = Player.CharacterManager.Instance.characterList[activeCharIndex];

            if (Player.CharacterManager.Instance.UnlockCharacter(charData.characterId))
            {
                UpdateCharacterPreview();
            }
        }

        public void OnClickSelect()
        {
            if (Player.CharacterManager.Instance == null) return;
            var charData = Player.CharacterManager.Instance.characterList[activeCharIndex];
            Player.CharacterManager.Instance.selectedCharacterId = charData.characterId;

            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.selectedCharacter = charData.characterId;
                Core.SaveManager.Instance.SaveGame();
            }

            CloseCharacterSelection();
        }
    }
}

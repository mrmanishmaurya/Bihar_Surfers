using UnityEngine;

namespace BiharSurfers.Data
{
    public enum MissionRewardType { Coins, XP, CharacterUnlock, SkinUnlock }

    [CreateAssetMenu(fileName = "NewMissionData", menuName = "BiharSurfers/Mission Data")]
    public class MissionData : ScriptableObject
    {
        public string missionId = "m1";
        public string title = "Rupee Collector";
        [TextArea] public string description = "Collect 100 golden Rupee coins.";
        public int targetAmount = 100;
        public int currentAmount = 0;

        [Header("Rewards")]
        public MissionRewardType rewardType = MissionRewardType.Coins;
        public int rewardCoinAmount = 500;
        public int rewardXPAmount = 100;
        public string rewardUnlockId = ""; // Character ID or Skin ID if rewardType is unlock

        public bool isCompleted = false;
        public bool isClaimed = false;
    }
}

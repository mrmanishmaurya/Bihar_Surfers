using UnityEngine;

namespace BiharSurfers.Data
{
    [CreateAssetMenu(fileName = "NewLocationData", menuName = "BiharSurfers/Location Data")]
    public class LocationData : ScriptableObject
    {
        [Header("Location Identity")]
        public int locationId;
        public string districtName = "Patna";
        public string stationName = "Patna Junction";
        public string stationCode = "PNBE";
        public string hindiName = "पटना जंक्शन";
        public string englishName = "PATNA JUNCTION";

        [Header("Progression Threshold")]
        public float unlockDistance = 0f; // Distance threshold in meters

        [Header("Visual Theme & Lighting")]
        public string environmentTheme = "UrbanRailway"; // UrbanRailway, TownStation, GangaRiver, HistoricalFort, RailMarket, SpiritualTemple, LitchiBazaar, MithilaPalace, MadhubaniArt, SilkCity
        public Color primaryThemeColor = new Color(0.9f, 0.5f, 0.1f);
        public Color fogColor = new Color(0.7f, 0.8f, 0.9f);
        public Color ambientLightColor = Color.white;
        public Material skyboxMaterial;

        [Header("Location Signs & Decor")]
        public string stationSignText = "PATNA JUNCTION - PNBE";
        public GameObject[] environmentDecorPrefabs;
        public string[] obstacleThemeKeys;

        [TextArea] public string locationDescription = "Capital city railway station with grand architecture and busy platforms.";
    }
}

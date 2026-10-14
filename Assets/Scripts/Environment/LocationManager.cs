using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Environment
{
    /// <summary>
    /// Master location progression manager handling transitions across 10 Bihar locations:
    /// Patna Junction, Ara, Buxar, Sasaram Junction, Chhapra, Gaya, Muzaffarpur, Darbhanga, Madhubani, Bhagalpur.
    /// Monitors player distance run and triggers location transitions.
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        public static LocationManager Instance { get; private set; }

        [Header("Location Data Registry")]
        public List<Data.LocationData> locationList = new List<Data.LocationData>();
        public int currentLocationIndex = 0;

        [Header("Progression Distance Interval")]
        public float defaultIntervalMeters = 800f; // Transition every 800 meters

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeDefaultLocations();
        }

        private void InitializeDefaultLocations()
        {
            if (locationList.Count > 0) return;

            // 1. Patna Junction (PNBE)
            var loc1 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc1.locationId = 1;
            loc1.districtName = "Patna";
            loc1.stationName = "Patna Junction";
            loc1.stationCode = "PNBE";
            loc1.hindiName = "पटना जंक्शन";
            loc1.englishName = "PATNA JUNCTION";
            loc1.unlockDistance = 0f;
            loc1.environmentTheme = "UrbanRailway";
            loc1.fogColor = new Color(0.85f, 0.75f, 0.65f); // Warm sunrise fog
            loc1.stationSignText = "PATNA JUNCTION - PNBE";
            locationList.Add(loc1);

            // 2. Ara Junction (ARA)
            var loc2 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc2.locationId = 2;
            loc2.districtName = "Ara";
            loc2.stationName = "Ara Junction";
            loc2.stationCode = "ARA";
            loc2.hindiName = "आरा जंक्शन";
            loc2.englishName = "ARA JUNCTION";
            loc2.unlockDistance = 500f;
            loc2.environmentTheme = "TownStation";
            loc2.fogColor = new Color(0.75f, 0.85f, 0.7f); // Town green tint
            loc2.stationSignText = "ARA JUNCTION - ARA";
            locationList.Add(loc2);

            // 3. Buxar (BXR)
            var loc3 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc3.locationId = 3;
            loc3.districtName = "Buxar";
            loc3.stationName = "Buxar";
            loc3.stationCode = "BXR";
            loc3.hindiName = "बक्सर";
            loc3.englishName = "BUXAR";
            loc3.unlockDistance = 1000f;
            loc3.environmentTheme = "GangaRiver";
            loc3.fogColor = new Color(0.6f, 0.8f, 0.95f); // River Ganga blue fog
            loc3.stationSignText = "BUXAR - BXR";
            locationList.Add(loc3);

            // 4. Sasaram Junction (SSM)
            var loc4 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc4.locationId = 4;
            loc4.districtName = "Sasaram";
            loc4.stationName = "Sasaram Junction";
            loc4.stationCode = "SSM";
            loc4.hindiName = "सासाराम जंक्शन";
            loc4.englishName = "SASARAM JUNCTION";
            loc4.unlockDistance = 1500f;
            loc4.environmentTheme = "HistoricalFort";
            loc4.fogColor = new Color(0.7f, 0.7f, 0.6f); // Historic stone fog
            loc4.stationSignText = "SASARAM JUNCTION - SSM";
            locationList.Add(loc4);

            // 5. Chhapra Junction (CPR)
            var loc5 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc5.locationId = 5;
            loc5.districtName = "Chhapra";
            loc5.stationName = "Chhapra Junction";
            loc5.stationCode = "CPR";
            loc5.hindiName = "छपरा जंक्शन";
            loc5.englishName = "CHHAPRA JUNCTION";
            loc5.unlockDistance = 2000f;
            loc5.environmentTheme = "RailMarket";
            loc5.fogColor = new Color(0.9f, 0.8f, 0.7f); // Market amber fog
            loc5.stationSignText = "CHHAPRA JUNCTION - CPR";
            locationList.Add(loc5);

            // 6. Muzaffarpur Junction (MFP)
            var loc6 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc6.locationId = 6;
            loc6.districtName = "Muzaffarpur";
            loc6.stationName = "Muzaffarpur Junction";
            loc6.stationCode = "MFP";
            loc6.hindiName = "मुजफ्फरपुर जंक्शन";
            loc6.englishName = "MUZAFFARPUR JUNCTION";
            loc6.unlockDistance = 2500f;
            loc6.environmentTheme = "LitchiBazaar";
            loc6.fogColor = new Color(0.95f, 0.75f, 0.8f);
            loc6.stationSignText = "MUZAFFARPUR JUNCTION - MFP";
            locationList.Add(loc6);

            // 7. Darbhanga Junction (DBG)
            var loc7 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc7.locationId = 7;
            loc7.districtName = "Darbhanga";
            loc7.stationName = "Darbhanga Junction";
            loc7.stationCode = "DBG";
            loc7.hindiName = "दरभंगा जंक्शन";
            loc7.englishName = "DARBHANGA JUNCTION";
            loc7.unlockDistance = 3000f;
            loc7.environmentTheme = "MithilaPalace";
            loc7.fogColor = new Color(0.8f, 0.85f, 0.95f);
            loc7.stationSignText = "DARBHANGA JUNCTION - DBG";
            locationList.Add(loc7);

            // 8. Samastipur Junction (SPJ)
            var loc8 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc8.locationId = 8;
            loc8.districtName = "Samastipur";
            loc8.stationName = "Samastipur Junction";
            loc8.stationCode = "SPJ";
            loc8.hindiName = "समस्तीपुर जंक्शन";
            loc8.englishName = "SAMASTIPUR JUNCTION";
            loc8.unlockDistance = 3500f;
            loc8.environmentTheme = "CentralHub";
            loc8.fogColor = new Color(0.85f, 0.85f, 0.75f);
            loc8.stationSignText = "SAMASTIPUR JUNCTION - SPJ";
            locationList.Add(loc8);

            // 9. Gaya Junction (GAYA)
            var loc9 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc9.locationId = 9;
            loc9.districtName = "Gaya";
            loc9.stationName = "Gaya Junction";
            loc9.stationCode = "GAYA";
            loc9.hindiName = "गया जंक्शन";
            loc9.englishName = "GAYA JUNCTION";
            loc9.unlockDistance = 4000f;
            loc9.environmentTheme = "SpiritualTemple";
            loc9.fogColor = new Color(0.85f, 0.7f, 0.9f);
            loc9.stationSignText = "GAYA JUNCTION - GAYA";
            locationList.Add(loc9);

            // 10. Purnea Junction (PRNA)
            var loc10 = ScriptableObject.CreateInstance<Data.LocationData>();
            loc10.locationId = 10;
            loc10.districtName = "Purnea";
            loc10.stationName = "Purnea Junction";
            loc10.stationCode = "PRNA";
            loc10.hindiName = "पूर्णिया जंक्शन";
            loc10.englishName = "PURNEA JUNCTION";
            loc10.unlockDistance = 4500f;
            loc10.environmentTheme = "GreenValleys";
            loc10.fogColor = new Color(0.75f, 0.9f, 0.8f);
            loc10.stationSignText = "PURNEA JUNCTION - PRNA";
            locationList.Add(loc10);
        }

        private void Update()
        {
            if (Core.ScoreManager.Instance == null || Core.GameManager.Instance == null || Core.GameManager.Instance.currentState != Core.GameState.Playing)
                return;

            float currentDistance = Core.ScoreManager.Instance.currentDistance;

            // Check if player has crossed threshold for next location
            if (currentLocationIndex + 1 < locationList.Count)
            {
                var nextLoc = locationList[currentLocationIndex + 1];
                if (currentDistance >= nextLoc.unlockDistance)
                {
                    currentLocationIndex++;
                    ApplyLocationTransition(nextLoc);
                }
            }
        }

        public Data.LocationData GetCurrentLocation()
        {
            if (locationList.Count == 0) return null;
            return locationList[Mathf.Clamp(currentLocationIndex, 0, locationList.Count - 1)];
        }

        public void ApplyLocationTransition(Data.LocationData location)
        {
            if (location == null) return;

            // Trigger Transition Banner & Lighting
            LocationTransition.Instance?.TriggerLocationTransition(location);

            // Save current district index
            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.currentDistrictIndex = currentLocationIndex;
                Core.SaveManager.Instance.SaveGame();
            }

            // Check location achievement
            if (currentLocationIndex >= 4)
            {
                Core.AchievementManager.Instance?.Unlock("ach_explorer");
            }
        }

        public void ResetLocationProgression()
        {
            currentLocationIndex = 0;
            if (locationList.Count > 0)
            {
                ApplyLocationTransition(locationList[0]);
            }
        }
    }
}

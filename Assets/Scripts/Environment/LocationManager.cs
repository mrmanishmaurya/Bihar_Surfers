using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Environment
{
    /// <summary>
    /// Master location progression manager handling transitions across all 38 official districts of Bihar:
    /// Patna, Nalanda, Gaya, Nawada, Aurangabad, Jehanabad, Arwal, Rohtas, Kaimur, Buxar,
    /// Bhojpur (Ara), Saran (Chhapra), Siwan, Gopalganj, Vaishali (Hajipur), Muzaffarpur, Sheohar, Sitamarhi,
    /// East Champaran (Motihari), West Champaran (Bettiah), Darbhanga, Madhubani, Samastipur, Begusarai,
    /// Munger, Lakhisarai, Sheikhpura, Jamui, Khagaria, Bhagalpur, Banka, Saharsa,
    /// Supaul, Madhepura, Purnia, Kishanganj, Araria, and Katihar.
    ///
    /// Tracks distance milestones (1,000 meters per district), enables district selection,
    /// and triggers visual environmental transitions.
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        public static LocationManager Instance { get; private set; }

        [Header("Location Data Registry")]
        public List<Data.LocationData> locationList = new List<Data.LocationData>();
        public int currentLocationIndex = 0;
        public int startingDistrictIndex = 0;

        [Header("Progression Distance Interval")]
        public float distancePerDistrictMeters = 1000f;

        public System.Action<Data.LocationData> OnLocationChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeAll38Districts();
        }

        private void InitializeAll38Districts()
        {
            if (locationList.Count > 0) return;

            var districtDefs = new (string district, string station, string code, string hindi, string theme, Color col, Color fog, string desc)[]
            {
                ("Patna", "Patna Junction", "PNBE", "पटना जंक्शन", "UrbanRailway", new Color(0.95f, 0.48f, 0.12f), new Color(0.85f, 0.78f, 0.70f), "Capital city junction with bustling platforms, grand colonial architecture and Ganga riverfront views."),
                ("Nalanda", "Nalanda Station", "NLD", "नालंदा स्टेशन", "HistoricalFort", new Color(0.82f, 0.35f, 0.15f), new Color(0.80f, 0.70f, 0.60f), "Ancient seat of world learning with terracotta brick ruins, monasteries, and stupa architecture."),
                ("Gaya", "Gaya Junction", "GAYA", "गया जंक्शन", "SpiritualTemple", new Color(0.92f, 0.70f, 0.20f), new Color(0.75f, 0.75f, 0.85f), "Spiritual sanctuary surrounded by hills, Vishnupad temple, and Bodh Gaya pilgrimage paths."),
                ("Nawada", "Nawada Station", "NWD", "नवादा स्टेशन", "ForestTrail", new Color(0.35f, 0.65f, 0.30f), new Color(0.70f, 0.85f, 0.75f), "Picturesque landscape near Kakolat waterfalls, lush valley greenery, and stone hills."),
                ("Aurangabad", "Anugrah Narayan Road", "AUBR", "अनुग्रह नारायण रोड", "HistoricalFort", new Color(0.78f, 0.40f, 0.22f), new Color(0.85f, 0.80f, 0.70f), "Historic Sun Temple city of Deo with ancient rock inscriptions and Grand Trunk road crossings."),
                ("Jehanabad", "Jehanabad Station", "JHD", "जहानाबाद स्टेशन", "TownStation", new Color(0.60f, 0.55f, 0.45f), new Color(0.78f, 0.78f, 0.80f), "Fertile heartland station connecting agricultural plains and ancient Barabar rock-cut caves."),
                ("Arwal", "Arwal City Crossing", "ARWL", "अरवल स्टेशन", "RuralCrossing", new Color(0.45f, 0.60f, 0.35f), new Color(0.80f, 0.85f, 0.75f), "Son river basin crossing featuring scenic canal routes, bamboo groves, and open mustard fields."),
                ("Rohtas", "Sasaram Junction", "SSM", "सासाराम जंक्शन", "HistoricalFort", new Color(0.75f, 0.35f, 0.25f), new Color(0.82f, 0.75f, 0.65f), "Home of Sher Shah Suri's magnificent red sandstone mausoleum surrounded by a grand lotus lake."),
                ("Kaimur", "Bhabua Road Station", "BBU", "भभुआ रोड स्टेशन", "ForestTrail", new Color(0.30f, 0.58f, 0.35f), new Color(0.68f, 0.80f, 0.70f), "Scenic Kaimur wildlife hills, Telhar waterfall forests, and ancient Mundeshwari stone temple trails."),
                ("Buxar", "Buxar Station", "BXR", "बक्सर स्टेशन", "GangaRiver", new Color(0.25f, 0.65f, 0.85f), new Color(0.70f, 0.82f, 0.90f), "Historic Ganga riverfront battlegrounds with tranquil ghats, palm groves, and riverside breeze."),
                ("Bhojpur", "Ara Junction", "ARA", "आरा जंक्शन", "TownStation", new Color(0.88f, 0.45f, 0.18f), new Color(0.85f, 0.78f, 0.70f), "Heart of Bhojpuri culture, historic Veer Kunwar Singh memorial grounds, and buzzing rail junctions."),
                ("Saran", "Chhapra Junction", "CPR", "छपरा जंक्शन", "RailMarket", new Color(0.85f, 0.30f, 0.25f), new Color(0.80f, 0.75f, 0.75f), "Grand multi-platform railway hub situated along the meeting point of Ganga and Ghaghara rivers."),
                ("Siwan", "Siwan Junction", "SV", "सीवान जंक्शन", "TownStation", new Color(0.55f, 0.62f, 0.72f), new Color(0.75f, 0.80f, 0.85f), "Birthplace of Dr. Rajendra Prasad with vibrant brass craft markets and rail interchanges."),
                ("Gopalganj", "Thawe Junction", "THE", "थावे जंक्शन", "SpiritualTemple", new Color(0.88f, 0.55f, 0.15f), new Color(0.85f, 0.80f, 0.70f), "Spiritual gateway renowned for the sacred Thawe Bhavani temple nestled among sacred forest groves."),
                ("Vaishali", "Hajipur Junction", "HJP", "हाजीपुर जंक्शन", "RuralCrossing", new Color(0.90f, 0.72f, 0.20f), new Color(0.88f, 0.85f, 0.75f), "World's earliest republic, famous Hajipur banana orchards, and Ashokan Lion Pillar monuments."),
                ("Muzaffarpur", "Muzaffarpur Junction", "MFP", "मुजफ्फरपुर जंक्शन", "LitchiBazaar", new Color(0.92f, 0.25f, 0.35f), new Color(0.88f, 0.75f, 0.78f), "The Litchi Kingdom with vibrant fruit bazaars, sweet aroma, and heavy rail traffic."),
                ("Sheohar", "Sheohar Crossing", "SHR", "शिवहर स्टेशन", "RuralCrossing", new Color(0.40f, 0.65f, 0.38f), new Color(0.78f, 0.84f, 0.76f), "Lush Bagmati river floodplains with pastoral village lanes, bamboo bridges, and mango orchards."),
                ("Sitamarhi", "Sitamarhi Junction", "SMI", "सीतामढ़ी जंक्शन", "SpiritualTemple", new Color(0.95f, 0.60f, 0.25f), new Color(0.85f, 0.80f, 0.75f), "Sacred birthplace of Goddess Sita featuring Punaura Dham and Janaki temple sanctums."),
                ("East Champaran", "Bapudham Motihari", "BMKI", "बापूधाम मोतिहारी", "HistoricalFort", new Color(0.85f, 0.60f, 0.35f), new Color(0.82f, 0.80f, 0.78f), "Mahatma Gandhi's historic Champaran Satyagraha memorial city and the grand Kesaria Stupa."),
                ("West Champaran", "Bettiah Station", "BTH", "बेतिया स्टेशन", "ForestTrail", new Color(0.28f, 0.55f, 0.32f), new Color(0.65f, 0.78f, 0.70f), "Valmiki National Park wilderness trails, tiger corridors, and Gandak river irrigation canals."),
                ("Darbhanga", "Darbhanga Junction", "DBG", "दरभंगा जंक्शन", "MithilaPalace", new Color(0.78f, 0.28f, 0.75f), new Color(0.80f, 0.75f, 0.85f), "Cultural capital of Mithila with ornate Raj Darbhanga royal palace gates, lakes, and music heritage."),
                ("Madhubani", "Madhubani Station", "MBI", "मधुबनी स्टेशन", "MadhubaniArt", new Color(0.95f, 0.30f, 0.40f), new Color(0.88f, 0.80f, 0.82f), "World-famous Mithila folk paintings adorning station walls, railway coaches, and village pavilions."),
                ("Samastipur", "Samastipur Junction", "SPJ", "समस्तीपुर जंक्शन", "UrbanRailway", new Color(0.40f, 0.50f, 0.75f), new Color(0.75f, 0.80f, 0.88f), "Major Indian Railways divisional headquarters with extensive locomotive yards and railway bridges."),
                ("Begusarai", "Barauni Junction", "BJU", "बरौनी जंक्शन", "UrbanRailway", new Color(0.65f, 0.45f, 0.35f), new Color(0.78f, 0.75f, 0.72f), "Industrial hub of Bihar with thermal power plant cooling towers, refineries, and Kanwar lake wetland."),
                ("Munger", "Jamalpur Junction", "JMP", "जमालपुर जंक्शन", "HistoricalFort", new Color(0.55f, 0.48f, 0.40f), new Color(0.76f, 0.74f, 0.72f), "Asia's oldest railway locomotive workshop founded in 1862, historic Munger fort, and Yoga university."),
                ("Lakhisarai", "Kiul Junction", "KIUL", "किऊल जंक्शन", "RailMarket", new Color(0.75f, 0.55f, 0.30f), new Color(0.82f, 0.78f, 0.74f), "Strategic railway interchange where Grand Chord and Main Line rail arteries converge."),
                ("Sheikhpura", "Sheikhpura Station", "SHK", "शेखपुरा स्टेशन", "RuralCrossing", new Color(0.68f, 0.58f, 0.42f), new Color(0.80f, 0.78f, 0.72f), "Quaint pastoral region surrounded by stone quarries, onion farms, and peaceful hillock backdrops."),
                ("Jamui", "Jamui Station", "JMU", "जमुई स्टेशन", "ForestTrail", new Color(0.35f, 0.52f, 0.38f), new Color(0.72f, 0.80f, 0.74f), "Jain pilgrimage sites at Lachhuar, scenic Nagi Dam bird sanctuary, and rolling forest hills."),
                ("Khagaria", "Khagaria Junction", "KGG", "खगड़िया जंक्शन", "GangaRiver", new Color(0.30f, 0.65f, 0.78f), new Color(0.74f, 0.82f, 0.88f), "The Land of Seven Rivers where Koshi, Bagmati, and Ganga rivers interlace across vast green floodplains."),
                ("Bhagalpur", "Bhagalpur Junction", "BGP", "भागलपुर जंक्शन", "SilkCity", new Color(0.88f, 0.40f, 0.60f), new Color(0.85f, 0.75f, 0.80f), "The Silk City of India, world-renowned for Tussar silk weaving, Vikramshila dolphin sanctuary, and university."),
                ("Banka", "Banka Junction", "BAKA", "बांका जंक्शन", "HistoricalFort", new Color(0.72f, 0.45f, 0.30f), new Color(0.80f, 0.76f, 0.70f), "Sacred Mandar Hill where mythological Samudra Manthan took place, surrounded by Chhath lake."),
                ("Saharsa", "Saharsa Junction", "SHC", "सहरसा जंक्शन", "TownStation", new Color(0.82f, 0.60f, 0.28f), new Color(0.84f, 0.80f, 0.74f), "Koshi division trade hub famous for Matsyagandha temple lake and Makhana wetland harvests."),
                ("Supaul", "Supaul Station", "SOU", "सुपौल स्टेशन", "RuralCrossing", new Color(0.42f, 0.68f, 0.45f), new Color(0.76f, 0.85f, 0.78f), "Koshi river delta with scenic bamboo farmhouses, water lily ponds, and fresh countryside air."),
                ("Madhepura", "Dauram Madhepura", "DMH", "मधेपुरा स्टेशन", "UrbanRailway", new Color(0.45f, 0.55f, 0.70f), new Color(0.78f, 0.80f, 0.85f), "Home of India's state-of-the-art high horsepower electric locomotive manufacturing facility."),
                ("Purnia", "Purnea Junction", "PRNA", "पूर्णिया जंक्शन", "TownStation", new Color(0.50f, 0.70f, 0.45f), new Color(0.75f, 0.84f, 0.78f), "Prosperous eastern trading center surrounded by maize granaries, jute fields, and broad gauge railways."),
                ("Kishanganj", "Kishanganj Station", "KNE", "किशनगंज स्टेशन", "ForestTrail", new Color(0.25f, 0.62f, 0.40f), new Color(0.70f, 0.85f, 0.75f), "Bihar's exclusive tea garden district with gentle rolling hills, misty mornings, and eastern border gateways."),
                ("Araria", "Araria Court", "ARQ", "अररिया कोर्ट स्टेशन", "RuralCrossing", new Color(0.60f, 0.65f, 0.40f), new Color(0.80f, 0.82f, 0.76f), "Literary heartland of Phanishwar Nath 'Renu', peaceful border plains, and rich cultural traditions."),
                ("Katihar", "Katihar Junction", "KIR", "कटिहार जंक्शन", "RailMarket", new Color(0.85f, 0.42f, 0.20f), new Color(0.82f, 0.76f, 0.70f), "Vast 8-line railway junction connecting Northeast India with Bihar, bustling jute mills, and wetland sanctuaries.")
            };

            for (int i = 0; i < districtDefs.Length; i++)
            {
                var d = districtDefs[i];
                var loc = ScriptableObject.CreateInstance<Data.LocationData>();
                loc.locationId = i + 1;
                loc.districtName = d.district;
                loc.stationName = d.station;
                loc.stationCode = d.code;
                loc.hindiName = d.hindi;
                loc.englishName = d.station.ToUpper();
                loc.unlockDistance = i * distancePerDistrictMeters;
                loc.environmentTheme = d.theme;
                loc.primaryThemeColor = d.col;
                loc.fogColor = d.fog;
                loc.ambientLightColor = Color.Lerp(Color.white, d.col, 0.25f);
                loc.stationSignText = $"{d.station.ToUpper()} ({d.code})";
                loc.locationDescription = d.desc;

                locationList.Add(loc);
            }
        }

        private void Update()
        {
            if (Core.ScoreManager.Instance == null || Core.GameManager.Instance == null || Core.GameManager.Instance.currentState != Core.GameState.Playing)
                return;

            float currentDistance = Core.ScoreManager.Instance.currentDistance;

            if (currentLocationIndex + 1 < locationList.Count)
            {
                var nextLoc = locationList[currentLocationIndex + 1];
                if (currentDistance >= nextLoc.unlockDistance)
                {
                    currentLocationIndex++;
                    ApplyLocationTransition(nextLoc);

                    if (Core.SaveManager.Instance != null)
                    {
                        Core.SaveManager.Instance.UnlockDistrict(currentLocationIndex);
                    }
                }
            }
        }

        public Data.LocationData GetCurrentLocation()
        {
            if (locationList.Count == 0) return null;
            return locationList[Mathf.Clamp(currentLocationIndex, 0, locationList.Count - 1)];
        }

        public void SelectStartingDistrict(int index)
        {
            if (index < 0 || index >= locationList.Count) return;

            startingDistrictIndex = index;
            currentLocationIndex = index;

            var loc = locationList[index];
            ApplyLocationTransition(loc);
        }

        public void ApplyLocationTransition(Data.LocationData location)
        {
            if (location == null) return;

            LocationTransition.Instance?.TriggerLocationTransition(location);

            RenderSettings.fog = true;
            RenderSettings.fogColor = location.fogColor;
            RenderSettings.fogDensity = 0.015f;

            if (Core.SaveManager.Instance != null)
            {
                Core.SaveManager.Instance.data.currentDistrictIndex = currentLocationIndex;
                Core.SaveManager.Instance.SaveGame();
            }

            OnLocationChanged?.Invoke(location);

            Core.AudioManager.Instance?.PlaySFX("DistrictUnlocked");

            if (currentLocationIndex >= 10)
            {
                Core.AchievementManager.Instance?.Unlock("ach_district_10");
            }
            if (currentLocationIndex >= 37)
            {
                Core.AchievementManager.Instance?.Unlock("ach_bihar_complete");
            }
        }

        public void ResetLocationProgression()
        {
            currentLocationIndex = startingDistrictIndex;
            if (locationList.Count > 0)
            {
                ApplyLocationTransition(locationList[currentLocationIndex]);
            }
        }
    }
}

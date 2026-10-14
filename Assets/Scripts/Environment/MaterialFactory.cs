using System.Collections.Generic;
using UnityEngine;

namespace BiharSurfers.Environment
{
    public enum BiharMaterialType
    {
        TrackGround,
        LaneMarking,
        SteelRail,
        WoodSleeper,
        SideGrass,
        SideDirt,
        Platform,
        StationSign,
        Barricade,
        RailwayBarrier,
        ConstructionBarrier,
        Coin,
        MagnetPowerUp,
        ShieldPowerUp,
        SpeedBoostPowerUp,
        PlayerBody,
        PlayerSkin,
        TreeFoliage,
        TreeTrunk,
        Building
    }

    /// <summary>
    /// Centralized material generator and manager. Ensures all procedural tracks,
    /// obstacles, coins, player visuals, and station environments receive vibrant,
    /// high-performance colored materials instead of default white primitives.
    /// </summary>
    public static class MaterialFactory
    {
        private static readonly Dictionary<BiharMaterialType, Material> materialCache = new Dictionary<BiharMaterialType, Material>();

        public static Material GetMaterial(BiharMaterialType matType)
        {
            if (materialCache.TryGetValue(matType, out Material cachedMat) && cachedMat != null)
            {
                return cachedMat;
            }

            Material mat = CreateMaterial(matType);
            materialCache[matType] = mat;
            return mat;
        }

        private static Material CreateMaterial(BiharMaterialType matType)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Mobile/Diffuse");
            if (shader == null) shader = Shader.Find("Unlit/Color");

            Material mat = new Material(shader);
            mat.name = $"Mat_{matType}";

            Color color = Color.white;
            float metallic = 0.0f;
            float smoothness = 0.2f;

            switch (matType)
            {
                case BiharMaterialType.TrackGround:
                    color = new Color(0.18f, 0.20f, 0.23f); // Dark Slate Railway Gravel
                    smoothness = 0.1f;
                    break;

                case BiharMaterialType.LaneMarking:
                    color = new Color(0.92f, 0.72f, 0.15f); // Bright Yellow Lane Marker
                    smoothness = 0.4f;
                    break;

                case BiharMaterialType.SteelRail:
                    color = new Color(0.65f, 0.68f, 0.72f); // Steel Rail Metal
                    metallic = 0.85f;
                    smoothness = 0.65f;
                    break;

                case BiharMaterialType.WoodSleeper:
                    color = new Color(0.28f, 0.21f, 0.16f); // Wooden Railway Tie Brown
                    smoothness = 0.1f;
                    break;

                case BiharMaterialType.SideGrass:
                    color = new Color(0.22f, 0.52f, 0.24f); // Vibrant Bihar Grass
                    smoothness = 0.1f;
                    break;

                case BiharMaterialType.SideDirt:
                    color = new Color(0.55f, 0.43f, 0.35f); // Bihar Earth Terracotta Dirt
                    smoothness = 0.05f;
                    break;

                case BiharMaterialType.Platform:
                    color = new Color(0.38f, 0.45f, 0.49f); // Station Platform Grey
                    smoothness = 0.2f;
                    break;

                case BiharMaterialType.StationSign:
                    color = new Color(0.98f, 0.75f, 0.18f); // Station Junction Yellow Signboard
                    smoothness = 0.5f;
                    break;

                case BiharMaterialType.Barricade:
                    color = new Color(0.98f, 0.55f, 0.0f); // Indian Road Barricade Orange
                    smoothness = 0.4f;
                    break;

                case BiharMaterialType.RailwayBarrier:
                    color = new Color(0.83f, 0.18f, 0.18f); // Railway Crossing Red Barrier
                    smoothness = 0.5f;
                    break;

                case BiharMaterialType.ConstructionBarrier:
                    color = new Color(0.99f, 0.85f, 0.21f); // Construction Caution Yellow
                    smoothness = 0.3f;
                    break;

                case BiharMaterialType.Coin:
                    color = new Color(1.0f, 0.84f, 0.0f); // Shiny Gold Rupee Coin
                    metallic = 0.9f;
                    smoothness = 0.8f;
                    break;

                case BiharMaterialType.MagnetPowerUp:
                    color = new Color(0.0f, 0.67f, 0.76f); // Chai Magnet Cyan
                    smoothness = 0.6f;
                    break;

                case BiharMaterialType.ShieldPowerUp:
                    color = new Color(0.85f, 0.26f, 0.08f); // Litti Shield Bronze
                    metallic = 0.5f;
                    smoothness = 0.5f;
                    break;

                case BiharMaterialType.SpeedBoostPowerUp:
                    color = new Color(1.0f, 0.34f, 0.13f); // Sattu Rocket Flame Red
                    smoothness = 0.7f;
                    break;

                case BiharMaterialType.PlayerBody:
                    color = new Color(0.0f, 0.4f, 1.0f); // Bright Blue Test Color
                    smoothness = 0.5f;
                    break;

                case BiharMaterialType.PlayerSkin:
                    color = new Color(0.88f, 0.71f, 0.58f); // Warm Skin Tone
                    smoothness = 0.2f;
                    break;

                case BiharMaterialType.TreeFoliage:
                    color = new Color(0.11f, 0.37f, 0.13f); // Deep Tree Green
                    smoothness = 0.1f;
                    break;

                case BiharMaterialType.TreeTrunk:
                    color = new Color(0.36f, 0.25f, 0.20f); // Tree Bark Brown
                    smoothness = 0.1f;
                    break;

                case BiharMaterialType.Building:
                    color = new Color(0.63f, 0.53f, 0.50f); // Terracotta Town Building
                    smoothness = 0.15f;
                    break;
            }

            if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            return mat;
        }

        public static void ApplyMaterial(GameObject obj, BiharMaterialType matType)
        {
            if (obj == null) return;

            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = GetMaterial(matType);
            }
        }
    }
}

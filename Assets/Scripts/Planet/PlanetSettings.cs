// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 6/15/2020
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace PcgUniverse2
{
    /// <summary>
    /// Holds planet settings so they can be either randomly generated OR 
    /// pre-designed in the case where we want specific features, like earth
    /// </summary>
    [CreateAssetMenu(menuName = "Planet Settings")]
    public class PlanetSettings : ScriptableObject
    {

        [SerializeField]
        private string m_planetName = "Planet X";
        public string planetName { get => m_planetName; set => m_planetName = value; }

        [SerializeField]
        private Planet.EPlanetType m_planetType = Planet.EPlanetType.kRocky;
        public Planet.EPlanetType planetType { get => m_planetType; set => m_planetType = value; }

        [SerializeField]
        private float m_radius = 2f;
        public float radius { get => m_radius; set => m_radius = value; }

        [SerializeField] private bool m_hasLiquidOcean = false;
        public bool hasLiquidOcean { get => m_hasLiquidOcean; set => m_hasLiquidOcean = value; }

        [SerializeField] private bool m_hasLife = false;
        public bool hasLife { get => m_hasLife; set => m_hasLife = value; }

        [Header("Biome Settings")]
        public Biome[] m_biomes;

        [Expandable]
        public NoiseSettings m_biomeNoiseSettings = null;
        public float m_biomeNoiseOffset = 0;
        public float m_biomeNoiseStrength = 1;
        [Range(0f, 1f)] public float m_biomeBlendAmount = 0.5f;

        [Header("Landmass Settings"), Expandable]
        public NoiseSettings[] m_noiseLayers = null;

        [SerializeField]
        private Dictionary<ElementColorDatabase.Element, float> m_geoElements = null;
        public Dictionary<ElementColorDatabase.Element, float> geoElements { get => m_geoElements; set => m_geoElements = value; }

        [SerializeField]
        private Dictionary<ElementColorDatabase.Element, float> m_atmosElements = null;
        public Dictionary<ElementColorDatabase.Element, float> atmosElements { get => m_atmosElements; set => m_atmosElements = value; }

        [Header("Atmosphere Settings")]
        [SerializeField] private float m_cloudThickness = 0.5f;
        public float cloudThickness { get => m_cloudThickness; set => m_cloudThickness = value; }

        [SerializeField] private bool m_thickAtmosphere = false;
        public bool thickAtmosphere { get => m_thickAtmosphere; set => m_thickAtmosphere = value; }
        [SerializeField] private Color m_atmoColor = Color.white;
        public Color atmoColor { get => m_atmoColor; }

        public void SaveToDisk()
        {
#if UNITY_EDITOR

            string randomNameMod = System.Guid.NewGuid().ToString().Substring(1, 6);
            string folder = $"Assets/Data/{ this.planetName } { randomNameMod }/";

            System.IO.Directory.CreateDirectory(folder);
            string filename = $"{ this.planetName }";
            // save landmass layers
            AssetDatabase.CreateAsset(m_noiseLayers[0], $"{ folder }/{ filename } Landmass.asset");
            AssetDatabase.CreateAsset(m_noiseLayers[1], $"{ folder }/{ filename } Mountains.asset");

            // save biome noise
            AssetDatabase.CreateAsset(m_biomeNoiseSettings, $"{ folder }/{ filename } Mountains.asset");

            // save overall asset
            AssetDatabase.CreateAsset(this, $"{ folder }/{ filename } Settings.asset");
            AssetDatabase.SaveAssets();
#endif

        }

        public void ClearElements()
        {
            m_geoElements.Clear();
            m_atmosElements.Clear();
        }

        public void AddWater()
        {

        }

        public void AddGeoElement(ElementColorDatabase.Element element, float amount)
        {
            if (m_geoElements == null)
                m_geoElements = new Dictionary<ElementColorDatabase.Element, float>();

            if (m_geoElements.ContainsKey(element))
                m_geoElements[element] += amount;
            else
                m_geoElements.Add(element, amount);
        }

        public void AddAtmosElement(ElementColorDatabase.Element element, float amount)
        {
            if (m_atmosElements == null)
                m_atmosElements = new Dictionary<ElementColorDatabase.Element, float>();

            if (m_atmosElements.ContainsKey(element))
                m_atmosElements[element] += amount;
            else
                m_atmosElements.Add(element, amount);
        }
    }

}


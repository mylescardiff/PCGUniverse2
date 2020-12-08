// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 9/18/2020
// ------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PcgUniverse2
{
    /// <summary>
    /// Holds all data for randomizing all three scenes
    /// </summary>
    [CreateAssetMenu(), System.Serializable]
    public class Randomizer : ScriptableObject
    {

        [SerializeField, Expandable]
        public ElementColorDatabase m_elementColorDb;

        [SerializeField]
        public NameGenerator m_nameGenerator;

        [Header("Stars")]
        public StarType[] m_starTypes;

        [Header("Planet Type & Size")]
        [Range(0f, 1f)]
        public float m_rockyPlanetChance = 0.85f;

        [Header("Features")]
        public float m_ringChance = 0.8f;
        public float m_minRingDistance = 4f;
        public float m_maxRingDistance = 10f;
        public float m_minRingDiameter = 0.1f;
        public float m_maxRingDiameter = 0.5f;

        [Header("Rocky Planets")]
        public float m_rockyMinSize = 10f;
        public float m_rockyMaxSize = 40f;

        [Expandable]
        public NoiseSettings m_rockyLandmassMinNoise;
        [Expandable]
        public NoiseSettings m_rockyLandmassMaxNoise;

        [Expandable]
        public NoiseSettings m_rockyMountiansMinNoise;
        [Expandable]
        public NoiseSettings m_rockyMountiansMaxNoise;

        [Range(0,1)]
        public float m_chanceOfLife = 0.1f;
        public Gradient m_oceanColors = null;

        public Gradient m_vegetationColors = null;

        public float m_chanceOfThickAtmos = 0.1f;
        public float m_minCloudThickness = 0.1f;
        public float m_maxCloudThickness = 0.8f;

        [Header("Gas Giants")]

        public float m_gasGiantMinSize = 50f;
        public float m_gasGiantMaxSize = 80f;

        public float m_gasGiantBiomeNoiseStrengthMin = 0.5f;
        public float m_gasGiantBiomeNoiseStrengthMax = 30f;

        public float m_nonOceanFloorLevel = 0.5f;


    }

}


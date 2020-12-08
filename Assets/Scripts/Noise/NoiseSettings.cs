// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 6/15/2020
// ------------------------------------------------------------------------------

using UnityEngine;

namespace PcgUniverse2
{

/// <summary>
/// Holds noise parameter data so that it can either be randomly generated or designed
/// </summary>
[CreateAssetMenu(menuName = "Noise Settings")]
public class NoiseSettings: ScriptableObject
{
    public enum NoiseType
    {
        Simple,
        Rigid
    }

    public bool m_enabled = true;
    public bool m_useFirstLayerAsMask = true;
    [SerializeField]
    private NoiseType m_type = NoiseType.Simple;
    [SerializeField, Range(0f, 5f)]
    private float m_strength = 0.01f;
    public float strength { get => m_strength; set => m_strength = value; }
    [SerializeField]
    private float m_baseRoughness = 1f;
    [SerializeField]
    private float m_roughness = 3f;
    [SerializeField]
    private float m_persistence = 0.5f;
    [SerializeField, Range(1,8)]
    private int m_octaves = 6;
    [SerializeField]
    private Vector3 m_center = Vector3.zero;
    [SerializeField, Range(0f, 1.5f)]
    private float m_minValue = 0.75f;
    public float minValue { get => m_minValue; set => m_minValue = value; }

    /// <summary>
    /// Initialze the noise data with some specific parameters
    /// </summary>
    public void Init(NoiseType type, float strength, float baseRoughness, float roughness, float persistence, int octaves, float minValue)
    {
        m_type = type;
        m_strength = strength;
        m_baseRoughness = baseRoughness;
        m_roughness = roughness;
        m_persistence = persistence;
        m_octaves = octaves;
        m_minValue = minValue;

        m_center = Vector3.zero;
        m_enabled = true;
    }

    /// <summary>
    /// Purely random values for all params
    /// </summary>
    public void Randomize()
    {
        m_strength = Random.Range(1f, 5f);
        m_baseRoughness = Random.Range(0f, 1f);
        m_roughness = Random.Range(1f, 2f);
        m_persistence = 0.5f;
        m_octaves = 1;
        m_minValue = 0;
    }

    /// <summary>
    /// Randomize noise based on external min and max settings
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void Randomize(NoiseSettings min, NoiseSettings max)
    {
        m_type = min.m_type;
        m_strength = Random.Range(min.m_strength, max.m_strength);
        m_baseRoughness = Random.Range(min.m_baseRoughness, max.m_baseRoughness);
        m_roughness = Random.Range(min.m_roughness, max.m_roughness);
        m_persistence = Random.Range(min.m_persistence, max.m_persistence);
        m_octaves = Random.Range(min.m_octaves, max.m_octaves);
        m_minValue = Random.Range(min.m_minValue, max.m_minValue);
    }

    /// <summary>
    /// Evaluates the noise and produces a float for use in height maps, color maps, etc.
    /// </summary>
    /// <param name="point">3D vector coordinate to calculate noise at</param>
    /// <param name="seed">Seed to use when generating noise</param>
    /// <returns></returns>
    public float Evaluate(Vector3 point, int seed)
    {
        Noise noise = new Noise(seed);
        float noiseValue = 0f;
        float frequency = m_baseRoughness;
        float amplitude = 1f;
        float weight = 1f;
        for (int i = 0; i < m_octaves; ++i)
        {
            float val = 0;
            if (m_type == NoiseType.Simple)
            {
                // simple
                val = noise.Evaluate(point * frequency + m_center);
                noiseValue += (val + 1f) * 0.5f * amplitude;
            }
            else
            {
                // rigid
                val = 1 - Mathf.Abs(noise.Evaluate(point * frequency + m_center));
                val *= val;
                val *= weight;
                weight = Mathf.Clamp01(val);
                noiseValue += val * amplitude;
            }
           
            frequency *= m_roughness;
            amplitude *= m_persistence;
        }

        // minimum floor (ocean, in earth's case)
        noiseValue = Mathf.Max(0, noiseValue - m_minValue);
        return noiseValue * m_strength;
    }

}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{

    [System.Serializable]
    public class StarType : IWeightedItem
    {
        private const float kUnityMagnitudeEqivilent = 250f;
        private const float kUnityAuConversion = 600f;

        private const float kBaseStarMagnitude = 4.77f; // Mv (absolute magnitute of Sol)
        private const float kBaseHabitableStart = 0.723f; // AU (habitable zone of Sol, start)
        private const float kBaseHabitableEnd = 1.523f; // AU (habitable zone of Sol, end)
        private const float kBaseTemperature = 6.5f;
        private const float kMaxTemperature = 25f;
        private const float kMaxOrbitDistance = 5000f;

        public string m_name;
        public Gradient m_surfaceColor;
        public Color m_coreColor;
        public Gradient m_ejectionColor;

        [Range(3.5f, kMaxTemperature)]
        public float m_temperature;

        [Range(0f, 1f)]
        public float m_abundance;

        public float m_minSize = 200f;
        public float m_maxSize = 300f;

        public float GetWeight()
        {
            return m_abundance;
        }

        /// <summary>
        /// The start of the habitable zome for this star
        /// </summary>
        /// <returns></returns>
        public float HabitableZoneStart()
        {
            float modifier = kUnityMagnitudeEqivilent / AbsoluteMagnitude();
            float habitableStart = kBaseHabitableStart * modifier * kUnityAuConversion;
            return habitableStart * HeatModifier() + 10f;
        }

        /// <summary>
        /// The start of the habitable zome for this star
        /// </summary>
        /// <returns></returns>
        public float HabitableZoneEnd()
        {
            float modifier = kUnityMagnitudeEqivilent / AbsoluteMagnitude();
            float habitableStart = kBaseHabitableEnd * modifier * kUnityAuConversion;
            return habitableStart * HeatModifier();
        }

        private float AbsoluteMagnitude()
        {
            return m_minSize + (m_maxSize - m_minSize) / 2;
        }
        private float HeatModifier()
        {
            return 1;
            //return (m_temperature / kBaseTemperature) / kMaxTemperature;
        }

    }
}

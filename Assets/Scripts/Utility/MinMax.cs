using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PcgUniverse2
{
    public class MinMax
    {
        private float m_min;
        private float m_max;

        public MinMax()
        {
            m_min = float.MaxValue;
            m_max = float.MinValue;
        }

        public void Add(float value)
        {
            if (value > m_max)
                m_max = value;

            if (value < m_min)
                m_min = value;

        }

        public float Min() { return m_min; }
        public float Max() { return m_max; }
    }

}

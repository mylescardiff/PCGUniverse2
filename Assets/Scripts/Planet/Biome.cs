// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 6/15/2020
// ------------------------------------------------------------------------------

using UnityEngine;

namespace PcgUniverse2
{
    /// <summary>
    /// Represents a colored area of the planet, typically in a stripe but noise can
    /// make it more interesting in shape
    /// </summary>
    [System.Serializable]
    public class Biome
    {
        public Biome()
        {
            m_gradient = new Gradient();
        }

        public Gradient m_gradient;
        [Range(0f, 1f)]
        public float startHeight;

    

    }

}

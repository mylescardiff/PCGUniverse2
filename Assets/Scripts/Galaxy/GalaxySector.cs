using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class GalaxySector
    {
        private int m_sectorId = 0;
        public int sectorId { get => m_sectorId; }

        private Vector3 m_center = Vector3.zero;
        public Vector3 center { get => m_center; }

        private List<GalaxyStar> m_stars = null;
        public List<GalaxyStar> stars { get => m_stars; set => m_stars = value; }

        public GalaxySector(int id, Vector3 center)
        {
            m_sectorId = id;
            m_center = center;

            m_stars = new List<GalaxyStar>();
        }
    }
}



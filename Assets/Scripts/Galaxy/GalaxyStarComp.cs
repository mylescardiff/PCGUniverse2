using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class GalaxyStarComp : MonoBehaviour
    {
        private GalaxyStar m_galaxyStar = null;
        public GalaxyStar galaxyStar { get => m_galaxyStar; set => m_galaxyStar = value; }
    }

}

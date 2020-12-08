using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    [CreateAssetMenu(menuName = "System Settings")]
    public class SystemSettings : ScriptableObject
    {
        [SerializeField] public int m_specialSeed = 1;
        [SerializeField] public StarType m_starType;
        [SerializeField] public List<PlanetSettings> m_planetSettings;
        


    }
}


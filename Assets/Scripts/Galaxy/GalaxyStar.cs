using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    public class GalaxyStar
    {
        private int m_seed = 0;
        public int seed { get => m_seed; set => m_seed = value; }
       
        private string m_name { get; set; }
        public string name { get => m_name; set => m_name = value; }

        private GameObject m_gameObject = null;
        public GameObject gameObject { get => m_gameObject; set => m_gameObject = value; }

        private Vector3 m_position = Vector3.zero;
        public Vector3 position { get => m_position; set => m_position = value; }

        private StarType m_starType = null;
        public StarType starType { get => m_starType; set => m_starType = value; }

        public StarNode m_node = null; 


        public void GenerateName()
        {
            NameGenerator nameGen = new NameGenerator();
            name = nameGen.Generate(m_seed);
        }
 
    }

}


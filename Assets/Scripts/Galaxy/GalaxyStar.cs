using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{

    /// <summary>
    /// Represents a star in the galaxy view. 
    /// </summary>
    public class GalaxyStar
    {

        /// <summary>
        /// This star's seed
        /// </summary>
        private int m_seed = 0;
        public int seed { get => m_seed; set => m_seed = value; }
       
        /// <summary>
        /// System name
        /// </summary>
        private string m_name { get; set; }
        public string name { get => m_name; set => m_name = value; }

        /// <summary>
        /// The game object linked to this node / star
        /// </summary>
        private GameObject m_gameObject = null;
        public GameObject gameObject { get => m_gameObject; set => m_gameObject = value; }

        /// <summary>
        /// The position in space 
        /// </summary>
        private Vector3 m_position = Vector3.zero;
        public Vector3 position { get => m_position; set => m_position = value; }

        /// <summary>
        /// The type of the star
        /// </summary>
        private StarType m_starType = null;
        public StarType starType { get => m_starType; set => m_starType = value; }

        /// <summary>
        /// Inidcator that the star has been discovered 
        /// </summary>
        private bool m_discovered = false;
        public bool discovered { get => m_discovered; set => m_discovered = value; }

        /// <summary>
        /// The graph node this star resides on, so it can be linked to other stars
        /// </summary>
        private StarNode m_node = null; 
        public StarNode node { get => m_node; set => m_node = value; }

        /// <summary>
        /// Uses the RNG to generate the name of the system based on its seed.
        /// </summary>
        public void GenerateName()
        {
            NameGenerator nameGen = new NameGenerator();
            name = nameGen.Generate(m_seed);
        }
 
    }

}


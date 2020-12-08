using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PcgUniverse2
{
    [CustomEditor(typeof(SolarSystem))]
    public class SolarSystemEditor : Editor
    {

        private SolarSystem m_system = null;

        private void OnEnable()
        {
            m_system = target as SolarSystem;
        }


        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Planets"))
                m_system.GeneratePlanets();
     
            DrawDefaultInspector();
       

        }
    }

}


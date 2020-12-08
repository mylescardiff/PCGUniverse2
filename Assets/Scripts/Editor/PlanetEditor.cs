using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace PcgUniverse2
{
    [CustomEditor(typeof(Planet))]
    public class PlanetEditor : Editor
    {
        Planet m_planet;

        private void OnEnable()
        {
            m_planet = target as Planet;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate Planet"))
                m_planet.GeneratePlanet();

            if (GUILayout.Button("Randomize"))
            {
                m_planet.RandomizeSeed();
                m_planet.GenerateBasics();
                m_planet.GenerateDetail();
                m_planet.GeneratePlanet();
            }
            if (GUILayout.Button("Save To Disk"))
            {
                m_planet.SavePlanetSettings();
            }
            DrawDefaultInspector();

        }
    }

}

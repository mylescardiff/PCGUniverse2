using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace PcgUniverse2
{

    [CustomEditor(typeof(Galaxy))]
    public class GalaxyEditor : Editor
    {
        Galaxy m_galaxy;
        Randomizer m_randomizer = null;

        private void OnEnable()
        {
            m_galaxy = target as Galaxy;
            m_randomizer = FindObjectOfType<Randomizer>();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Read Color DB"))
            {
                RandomizerComponent randComponent = GameObject.FindObjectOfType<RandomizerComponent>();
                m_randomizer = randComponent.m_randomizer;
                m_randomizer.m_elementColorDb.ReadCsv();
            }

        }
    }

}

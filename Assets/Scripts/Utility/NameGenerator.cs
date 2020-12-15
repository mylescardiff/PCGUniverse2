// ------------------------------------------------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 9/18/2020
// Disclaimer: Got some help on the math of the spiral from here: http://wiki.unity3d.com/index.php/Particle_Spiral_Effect
// ------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PcgUniverse2
{
    [System.Serializable]
    public class NameGenerator
    {
        private static string s_consonants = "bcdfghjklmnpqrstvwxyz";
        private static string s_vowells = "aeiou";

        [SerializeField, Range(0.01f, 1f)] private float m_sylableChanceReduction = 0.4f;
        [SerializeField, Range(0f, 1f)] private float m_chanceOfNewSylable = 0.8f;

        private string m_state = "";
        private int m_generation = 0;
        private float m_localSylableChance = 0f;

        public string Generate(int seed)
        {
            Reset();
            Random.InitState(seed);

            m_state = "SS";
            float roll = Random.value;

            while (NextGeneration())
            {
                ++m_generation;
            }

            // capitalize first letter of each word
            string temp = m_state;
            m_state = char.ToUpper(temp[0]).ToString();
            m_state += temp.Substring(1, temp.Length - 1);

            string returnValue = m_state;

            Reset();

            return returnValue;
        }

        private bool NextGeneration()
        {
            string newState = "";

            foreach (char ch in m_state)
            {
                if (ch == 'S')
                {
                    float roll = Random.value;
                    if (roll < m_localSylableChance)
                    {
                        newState += "SS";
                        m_localSylableChance -= m_sylableChanceReduction;
                    }
                    else
                    {
                        newState += "CV";
                    }
                }
                else if (ch == 'V')
                {
                    int randVowell = Random.Range(0, s_vowells.Length - 1);
                    newState += s_vowells[randVowell];
                }
                else if (ch == 'C')
                {
                    int randConstonant = Random.Range(0, s_consonants.Length - 1);
                    newState += s_consonants[randConstonant];
                }
                else
                {
                    newState += ch;
                }
            }
            m_state = newState;

            foreach (char ch in m_state)
                if (ch == 'V' || ch == 'C' || ch == 'S')
                    return true;

            return false;
        }

        public void Reset()
        {
            m_generation = 0;
            m_state = "SS";
            m_localSylableChance = m_chanceOfNewSylable;
        }

    }

}

// ------------------------------------
// Author: Myles Cardiff, myles@mylescardiff.com
// Created: 6/15/2020
// ------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PcgUniverse2
{
    /// <summary>
    /// Holds data about what color the different elements in the unverse
    /// should appear when rendered in the ocean, surface, or atmostphere
    /// </summary>
    [CreateAssetMenu()]
    public class ElementColorDatabase : ScriptableObject
    {
        public enum EElementType
        {
            kSolid,
            kLiquid,
            kGas, 
            kPlasma
        }
        [Serializable]
        public class Element: IWeightedItem
        {
            [SerializeField] private string m_name;
            public string name { get => m_name; set => m_name = value; }
        
            public int m_atomicNumber = 0;
            public string m_symbol;

            public Color m_color;
            public float m_weight = 0f;
            public EElementType m_type = EElementType.kSolid;
            public float GetWeight()
            {
                return m_weight;
            }

        }

        public List<Element> m_elements;

        [SerializeField]
        private TextAsset m_csvFile = null;

        public Element GetRandomElement(EElementType type)
        {
            Element[] elementsOfType = m_elements.Where(x => x.m_type == type).OrderByDescending(x => x.m_weight).ToArray();

            int randIndex =  Utility.WeightedRandom(elementsOfType);
            return elementsOfType[randIndex];
        }

      

        public void ReadCsv()
        {
            m_elements.Clear();
            m_elements = new List<Element>();

            string[] lines = m_csvFile.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                string[] lineSplitValues = line.Split(',');

                int atomicNumber = int.Parse(lineSplitValues[0]);
                string symbol = lineSplitValues[1];
                string name = lineSplitValues[2];
                float red = float.Parse(lineSplitValues[3]) / 255f;
                float green = float.Parse(lineSplitValues[4]) / 255f;
                float blue = int.Parse(lineSplitValues[5]) / 255f;
                float weight = float.Parse(lineSplitValues[6]);
                string typeString = lineSplitValues[7];

                EElementType typeEnum = EElementType.kSolid;
                if (typeString == "Gas")
                    typeEnum = EElementType.kGas;
                else if (typeString == "Liquid")
                    typeEnum = EElementType.kLiquid;
                else if (typeString == "Plasma")
                    typeEnum = EElementType.kPlasma;

                m_elements.Add(new Element
                {
                   m_atomicNumber = atomicNumber,
                   m_symbol = symbol,
                   name = name,
                   m_color = new Color(red, green, blue, 1f),
                   m_weight = weight,
                   m_type = typeEnum
                });

            }
        }
    }

}


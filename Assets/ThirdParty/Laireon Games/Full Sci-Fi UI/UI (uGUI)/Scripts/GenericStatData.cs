using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class GenericStatData : Object
    {

        // The Stat Name
        private string statName;
        public string StatName
        {
            get
            {
                return statName;
            }
            set
            {
                statName = value;
            }
        }

        // The Stat Value
        private int statValue;
        public int StatValue
        {
            get
            {
                return statValue;
            }
            set
            {
                statValue = value;
            }
        }

        // The Stat Format
        private string statFormat = "normal";
        public string StatFormat
        {
            get
            {
                return statFormat;
            }
            set
            {
                statFormat = value;
            }
        }
    }
}
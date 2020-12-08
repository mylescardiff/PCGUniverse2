using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class PieStatData : Object
    {

        // The Stat Name
        string statName;
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
        int statValue;
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

    }
}

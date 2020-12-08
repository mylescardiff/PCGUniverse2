using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class StatsBox : MonoBehaviour
    {
        public StatDisplay prefab;
        public Transform parent;
        StatDisplay[] statData;

        public void GenerateStatDisplay(GenericStatData[] data)
        {
            statData = new StatDisplay[data.Length];

            GameObject temp;

            for(int i = 0; i < data.Length; i++)
            {
                temp = GameObject.Instantiate(prefab.gameObject) as GameObject;
                temp.transform.SetParent(parent);
                temp.transform.localScale = Vector3.one;
                temp.transform.localPosition = Vector3.zero;

                statData[i] = temp.GetComponent<StatDisplay>();
                statData[i].InitStatDisplay(data[i].StatName, data[i].StatValue, data[i].StatFormat);
            }
        }

        public void RefreshStatsDisplay(float data)
        {
            for(int i = 0; i < statData.Length; i++)
                statData[i].RefreshDisplay(data);
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LaireonFramework
{
    public class LeaderboardsScreen : MonoBehaviour
    {
        public static LeaderboardsScreen Instance;

        public TransitionalObject mainTransition;

        public LeaderboardEntry entryPrefab;
        List<LeaderboardEntry> currentEntries = new List<LeaderboardEntry>();//this is currently a bit useless but it would be a good idea to pool the instances in some way depending on your circumstances

        public RectTransform container;//where to start spawning the entries and what to parent them to. The container is the mask that allows multiple entries
        public int spacing;

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);

            for(int i = 0; i < 40; i++)//here we are setting some placeholder data
                AddLeaderboardEntry(i, "Player " + i, (i * 2) + "", "00:0" + (i % 10) + ":00", (i * 10) + "");
        }

        /// <summary>
        /// Use this to add a new entry into the leaderboard
        /// </summary>
        public void AddLeaderboardEntry(int rank, string name, string multiplier, string time, string score)
        {
            GameObject temp = GameObject.Instantiate(entryPrefab.gameObject) as GameObject;
            temp.transform.SetParent(container);
            temp.transform.localScale = Vector3.one;
            temp.transform.localPosition = Vector3.zero;
            //temp.transform.localPosition = startPoint.localPosition + new Vector3(0,rank * (((RectTransform)entryPrefab.transform).GetHeight() +spacing) * -1, 0);
            currentEntries.Add(temp.GetComponent<LeaderboardEntry>());

            currentEntries[currentEntries.Count - 1].SetData(rank, name, multiplier, time, score);
        }

        public void Show()
        {
            mainTransition.TriggerTransition();
            MainMenu.Instance.Minimise();
            Shutters.Instance.Show();//also show the blue shutters
        }

        public void Close()
        {
            mainTransition.TriggerFadeOut();
            MainMenu.Instance.Show();
            Shutters.Instance.Close();
        }
    }
}
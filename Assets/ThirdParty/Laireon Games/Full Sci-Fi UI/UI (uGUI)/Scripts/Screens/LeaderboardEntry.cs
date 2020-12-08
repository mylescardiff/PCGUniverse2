using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    /// <summary>
    /// A single entry within the leaderboard
    /// </summary>
    public class LeaderboardEntry : MonoBehaviour
    {
        public Text rankLabel, nameLabel, multiplierLabel, timeLabel, scoreLabel;

        /// <summary>
        /// Sets the values of the labels to the current data, nothing fancy
        /// </summary>
        public void SetData(int rank, string name, string multiplier, string time, string score)
        {
            rankLabel.text = rank.ToString("D9");//gives a load of trailing 0's
            nameLabel.text = name;
            multiplierLabel.text = multiplier;
            timeLabel.text = time;
            scoreLabel.text = score;
        }
    }
}
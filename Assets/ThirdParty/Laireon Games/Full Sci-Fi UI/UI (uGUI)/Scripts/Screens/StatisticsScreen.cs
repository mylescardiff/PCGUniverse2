using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class StatisticsScreen : MonoBehaviour
    {
        #region Variables
        public static StatisticsScreen Instance;

        public TransitionGroup mainTransition;
        public TransitionalObject rightWindowTransition;//this window has elements added to it dynamically. So it needs to find the new elements when it animates alpha
        public TransitionalObject pieBackingTransition;

        public PieChart pieChart;
        public StatsBox statsBox;
        #endregion

        #region Methods
        void Start()
        {
            Instance = this;

            ReadExampleData();

            rightWindowTransition.InitialiseAlphaTransition();//basically now that we have created new UI items let the animations know there are new elements. This only applies to alpha and colour transitions

            gameObject.SetActive(false);//disable objects after initialising alpha, otherwise they cant be found!
        }

        public void ReadExampleData()
        {
            #region Pie Chart Data
            PieStatData slashKills = new PieStatData();
            slashKills.StatName = "Aliens killed with Slashes";
            slashKills.StatValue = 550;

            PieStatData tapKills = new PieStatData();
            tapKills.StatName = "Aliens killed with Taps";
            tapKills.StatValue = 275;

            PieStatData circleKills = new PieStatData();
            circleKills.StatName = "Aliens killed with Circles";
            circleKills.StatValue = 75;

            PieStatData droneKills = new PieStatData();
            droneKills.StatName = "Aliens killed with Drones";
            droneKills.StatValue = 420;

            // Generate stats for the Kill Chart
            pieChart.GeneratePieChart(new PieStatData[] { slashKills, tapKills, circleKills, droneKills });
            #endregion

            #region Stat Box Data
            GenericStatData bestScore = new GenericStatData();
            bestScore.StatName = "BEST SCORE";
            bestScore.StatValue = 250000000;

            GenericStatData bestMultiplier = new GenericStatData();
            bestMultiplier.StatName = "BEST MULTIPLIER";
            bestMultiplier.StatValue = 500;

            GenericStatData longestRun = new GenericStatData();
            longestRun.StatName = "LONGEST RUN";
            longestRun.StatValue = 26542;
            longestRun.StatFormat = "time";

            GenericStatData gamesPlayed = new GenericStatData();
            gamesPlayed.StatName = "GAMES PLAYED";
            gamesPlayed.StatValue = 72;

            GenericStatData totalTimePlayed = new GenericStatData();
            totalTimePlayed.StatName = "TOTAL TIME PLAYED";
            totalTimePlayed.StatValue = 254025;
            totalTimePlayed.StatFormat = "time";

            // Generate the stats for the score Box
            statsBox.GenerateStatDisplay(new GenericStatData[] { bestScore, bestMultiplier, longestRun, gamesPlayed, totalTimePlayed });
            #endregion
        }

        public void Show()
        {
            mainTransition.JumpToStart();
            mainTransition.TriggerGroupTransition(true);
            pieBackingTransition.JumpToStart();
            pieBackingTransition.Delay = -0.5f;//set a one shot delay so it doesnt start animating until the window  is in view
            pieBackingTransition.TriggerTransition();

            pieChart.Reset();//hides the chart whilst moving the screen into place
            MainMenu.Instance.Minimise();
            Shutters.Instance.Show();
        }

        public void Close()
        {
            mainTransition.TriggerGroupFadeOut();
            MainMenu.Instance.Show();
            Shutters.Instance.Close();
        }

        /// <summary>
        /// Called once UI element have moved into place. Used to initialise the pie chart
        /// </summary>
        public void Initialise()
        {
            pieChart.PlayPieChart();
            statsBox.RefreshStatsDisplay(1);
        }
        #endregion
    }
}

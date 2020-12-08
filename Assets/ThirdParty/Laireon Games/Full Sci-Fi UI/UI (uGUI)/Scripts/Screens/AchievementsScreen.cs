using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class AchievementsScreen : MonoBehaviour
    {
        public static AchievementsScreen Instance;

        public TransitionalObject mainTransition;

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);//initially hide the screen
        }

        public void Show()
        {
            Shutters.Instance.Show();
            mainTransition.TriggerTransition();
            MainMenu.Instance.Minimise();
        }

        public void Close()
        {
            Shutters.Instance.Close();
            mainTransition.TriggerFadeOut();
            MainMenu.Instance.Show();
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class CreditsScreen : MonoBehaviour
    {
        public static CreditsScreen Instance;
        public TransitionGroup mainTransition;
        public TransitionalObject creditTransition;
        public Text creditName, creditTitle;

        public string[] names, titles;

        int currentCredit;

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            mainTransition.TriggerGroupTransition();
            MainMenu.Instance.CloseUI();
            Shutters.Instance.Show();

            currentCredit = -1;
            LoadNextCredit();
        }

        public void Close()
        {
            mainTransition.TriggerGroupFadeOut();
            MainMenu.Instance.Show();
            Shutters.Instance.Close();
        }

        public void LoadNextCredit()
        {
            currentCredit++;

            if(currentCredit < names.Length)
            {
                creditName.text = names[currentCredit];
                creditTitle.text = titles[currentCredit];
                creditTransition.TriggerTransition();
            }
            else
                Close();
        }
    }
}

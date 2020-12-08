using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class UpgradesScreen : MonoBehaviour
    {
        public static UpgradesScreen Instance;

        public UpgradesButton upgradesButton;
        public TransitionGroup mainTransition;
        public Animator closeButton;//needed to kill the animations so the transitional object can edit the images

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
            closeButton.enabled = false;
        }

        public void Show()
        {
            mainTransition.TriggerGroupTransition();
            MainMenu.Instance.Minimise();
            Shutters.Instance.Show();
        }

        public void Close()
        {
            mainTransition.TriggerGroupFadeOut();
            MainMenu.Instance.Show();
            Shutters.Instance.Close();
            closeButton.enabled = false;
        }

        /// <summary>
        /// Called once the transitions have finished to allow user input
        /// </summary>
        public void EnableButtonPresses()
        {
            closeButton.enabled = true;
            closeButton.Play("Highlighted");
        }

        public void Purchase()
        {
            if(!upgradesButton.sufficientCredits)//placeholder check
                ConfirmDialogueScreen.Instance.Show();
        }
    }
}
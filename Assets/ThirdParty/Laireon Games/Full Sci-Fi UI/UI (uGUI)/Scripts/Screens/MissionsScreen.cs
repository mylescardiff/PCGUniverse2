using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class MissionsScreen : MonoBehaviour
    {
        public static MissionsScreen Instance;

        public TransitionalObject mainTransition;
        public Animator closeButton;//needed to kill the animations so the transitional object can edit the images


        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
            closeButton.enabled = false;
        }

        public void Show()
        {
            mainTransition.TriggerTransition();
            Shutters.Instance.Show();
            MainMenu.Instance.Minimise();
        }

        public void Close()
        {
            mainTransition.TriggerFadeOut();
            Shutters.Instance.Close();
            MainMenu.Instance.Show();
            closeButton.CrossFade("Normal", 0);
            closeButton.Update(1f);

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
    }
}
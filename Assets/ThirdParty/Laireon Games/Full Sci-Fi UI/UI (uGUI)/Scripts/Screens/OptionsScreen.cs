using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class OptionsScreen : MonoBehaviour
    {
        public static OptionsScreen Instance;

        public TransitionalObject mainTransition;

        public Text musicVolumeLabel, soundEffectsVolumeLabel;
        public Slider musicVolumeSlider, soundEffectsVolumeSlider;
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

        public void SetMusicVolume()
        {
            musicVolumeLabel.text = Mathf.RoundToInt(musicVolumeSlider.value) + "%";

            //Set music volume here!
        }

        public void SetSoundEffectsVolume()
        {
            soundEffectsVolumeLabel.text = Mathf.RoundToInt(soundEffectsVolumeSlider.value) + "%";

            //Set sound effects volume here!
        }
    }
}
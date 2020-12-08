using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class RadialFillEffect : MonoBehaviour
    {
        float timer = 0f;  // The Timer
        public float timerLimit = 20f; // The Timer Limit
        public bool timerActive = true;// Timer Tracker

        public Image image;  // Target image

        void Update()
        {
            if(timerActive)// If the timer is active
            {
                timer += Time.deltaTime;// Increment the timer

                if(timer >= timerLimit)// If the limit is reached
                    timer = 0; // Reset the timer
                else
                    UpdateFill();// Alter the fill
            }
        }

        /// <summary>
        /// Function updates the visual appearance of the fill
        /// </summary>
        void UpdateFill()
        {
            image.fillAmount = (timer / timerLimit);        // Setting the Fill amount
            image.color = new Color(image.color.r, image.color.g, image.color.b, (1f - (timer / timerLimit))); // Setting the alpha
        }
    }
}

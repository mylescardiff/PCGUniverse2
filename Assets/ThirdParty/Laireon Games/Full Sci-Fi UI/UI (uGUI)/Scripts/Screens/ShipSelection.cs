using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaireonFramework
{
    public class ShipSelection : MonoBehaviour
    {
        public static ShipSelection Instance;
        public TransitionGroup mainTransition;
        public CharacterCarousel carousel;

        [Tooltip("This should match the delay of your transition")]
        public float carouselDelay;

        void Start()
        {
            gameObject.SetActive(false);
            Instance = this;
            carousel.Initialise();
        }

        public void Show()
        {
            mainTransition.TriggerGroupTransition();
            carousel.Reset(carouselDelay);
        }

        public void Close()
        {
            mainTransition.TriggerGroupFadeOut();
        }

        public void SelectedShip()
        {
            Close();
            HUD.Instance.Show();
        }
    }

}
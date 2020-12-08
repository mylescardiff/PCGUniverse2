using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class Shutters : MonoBehaviour
    {
        public static Shutters Instance;

        public RectTransform[] shutters;

        public TransitionalObject mainTransition;

        void Start()
        {
            Instance = this;
            gameObject.SetActive(false);
            enabled = false;
        }

        public void Show()
        {
            mainTransition.TriggerTransition();
            enabled = true;
            gameObject.SetActive(true);
        }

        public void Close()
        {

            mainTransition.TriggerFadeOut();
            enabled = true;//run the update function. Note that this is disabled by a message on the transitionalObject as a small optimisation
        }

        public void FinishedClosing()
        {
            enabled = false;
            Update();//run a final update
            gameObject.SetActive(false);
        }

        void Update()
        {
            for(int i = 0; i < shutters.Length; i++)
                shutters[i].SetHeight(450 * mainTransition.transitions[0].CurrentValue);
        }
    }
}
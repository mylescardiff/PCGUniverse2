using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class HUD : MonoBehaviour
    {
        public static HUD Instance;

        public TransitionGroup transitionGroup;

        void Start()
        {
            Instance = this;

            transitionGroup.JumpToStart();//hide the HUD for the start
            gameObject.SetActive(false);//can hide this now to save some processing
        }

        public void Show()
        {
            transitionGroup.TriggerGroupTransition();
        }

        public void Close()
        {
            transitionGroup.TriggerGroupFadeOut();
        }
    }
}
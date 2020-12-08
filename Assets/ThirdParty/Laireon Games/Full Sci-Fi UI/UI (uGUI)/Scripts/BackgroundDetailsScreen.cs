using UnityEngine;
using System.Collections;

namespace LaireonFramework
{
    public class BackgroundDetailsScreen : MonoBehaviour
    {
        public static BackgroundDetailsScreen Instance;

        public TransitionalObject mainTransition, loopingTransition;//the looping transition is one that need to be handled seperately to stop and restart its looping

        void Start()
        {
            Instance = this;
        }

        public void Show()
        {
            mainTransition.TriggerTransition();
            loopingTransition.Delay = -0.5f;//this is a helper to give a smooth transition. A negative delay will cause a delay to run once then set to 0. this means the images fades in nicely and then does its pulse. 
                                            //otherwise the pulse animation would just overwrite the parent one. I am planning a future update to automatically detect and fix these scenarios

            loopingTransition.TriggerTransition();
        }

        public void Hide()
        {
            mainTransition.TriggerFadeOut();
            loopingTransition.Stop();//stop the looping, overwise the transitions would clash since both are trying to edit the images colour!
        }
    }
}
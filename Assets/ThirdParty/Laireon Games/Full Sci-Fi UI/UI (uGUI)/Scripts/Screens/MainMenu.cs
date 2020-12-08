using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class MainMenu : MonoBehaviour
    {
        public static MainMenu Instance;

        /// <summary>
        /// Transitions for opening and closing the GUI. Each element is a UI component
        /// </summary>
        public StateBasedTransition[] transitions;

        public TransitionalObject fadeTransition, minimiseTransition;//this fades all objects in the menu. The minimise is subtly different where it fades teh screen to half alpha

        void Start()
        {
            Instance = this;

            ResetUI();
        }

        public void ResetUI()
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].SwitchState(StateBasedTransition.State.Closing);

            Show();
        }

        public void Show()
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].SwitchState(StateBasedTransition.State.Opening);

            if(fadeTransition.transitions[0].state == TransitionalObjects.BaseTransition.TransitionState.Finished)//if the menu was faded out
                fadeTransition.TriggerTransition();
            else if(minimiseTransition.transitions[0].state == TransitionalObjects.BaseTransition.TransitionState.Waiting)//if the menu was faded out
                minimiseTransition.TriggerFadeOut();

            StartCoroutine(Initialise());
        }

        IEnumerator Initialise()
        {
            yield return new WaitForSeconds(fadeTransition.transitions[0].transitionInTime);//wait until the transition has finished

            //NewsTicker.Instance.StartBroadcast();
        }

        public void CloseUI()
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].SwitchState(StateBasedTransition.State.Closing);

            fadeTransition.TriggerFadeOut();

            NewsTicker.Instance.StopBroadcast();
        }

        public void Minimise()
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].SwitchState(StateBasedTransition.State.Minimising);

            minimiseTransition.TriggerTransition();

            NewsTicker.Instance.StopBroadcast();
        }
    }
}
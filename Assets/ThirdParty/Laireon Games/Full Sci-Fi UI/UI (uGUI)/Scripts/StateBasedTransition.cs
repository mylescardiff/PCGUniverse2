using UnityEngine;
using System.Collections;
using TransitionalObjects;

namespace LaireonFramework
{
    /// <summary>
    /// A simple example using the TransitionalObject to handle multiple states
    /// </summary>
    public class StateBasedTransition : MonoBehaviour
    {
        public enum State { Opening = 0, Closing, Minimising }

        public TransitionalObject transition;

        public Vector3 openingPosition, closingPosition, minimisingPosition;

        public void SwitchState(State state)
        {
            switch(state)
            {
                case State.Opening:
                    ((MovingTransition)transition.transitions[0]).startPoint = transform.localPosition;
                    ((MovingTransition)transition.transitions[0]).endPoint = openingPosition;
                    transition.transitions[0].transitionInTime = 0.8f;

                    transition.TriggerTransition();
                    break;

                case State.Closing:
                    ((MovingTransition)transition.transitions[0]).endPoint = transform.localPosition;
                    ((MovingTransition)transition.transitions[0]).startPoint = closingPosition;
                    transition.transitions[0].transitionInTime = 0.8f;

                    transition.TriggerFadeOut();
                    break;

                case State.Minimising:
                    ((MovingTransition)transition.transitions[0]).startPoint = transform.localPosition;
                    ((MovingTransition)transition.transitions[0]).endPoint = minimisingPosition;

                    transition.TriggerTransition();
                    break;
            }
        }
    }
}
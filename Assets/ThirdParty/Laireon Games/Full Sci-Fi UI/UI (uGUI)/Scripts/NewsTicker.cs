using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class NewsTicker : MonoBehaviour
    {
        public static NewsTicker Instance;

        public string[] newsItems;//these are just examples you can use

        // Reference to the ticker label
        public Text tickerLabel;

        public TransitionalObject tickerTransition;

        /// <summary>
        /// Which message is currently being displayed
        /// </summary>
        int currentMessage = -1;

        // Customization Options //
        // --------------------- //
        // If enabled, the ticker will loop through messages held in the array list.
        public bool loopMessages = true;
        // How long between ticker postings?
        public float waitTime = 0f;
        // How long will the message pause for?
        public float pauseTime = 2f;


        void Start()
        {
            Instance = this;

            tickerTransition.transitions[1].delay = pauseTime + tickerTransition.transitions[0].transitionInTime;//this is just an example of where to find values to edit. The values can just as easily be set in the editor

            LoadNextMessage();
        }

        // Function resets the ticker to the starting point.
        void ResetTicker()
        {
            tickerTransition.transitions[0].JumpToStart();
            tickerTransition.transitions[1].Stop();
        }

        public void LoadNextMessage()
        {
            if(waitTime > 0)
                StartCoroutine(DelayNextMessage());
            else
                ActuallyLoadNextMessage();
        }

        IEnumerator DelayNextMessage()
        {
            yield return new WaitForSeconds(waitTime);
            ActuallyLoadNextMessage();
        }

        void ActuallyLoadNextMessage()
        {
            currentMessage++;

            if(currentMessage >= newsItems.Length)
                if(loopMessages)
                    currentMessage = 0;
                else
                    return;

            tickerLabel.text = newsItems[currentMessage];//load the next message

            tickerTransition.transitions[1].state = TransitionalObjects.BaseTransition.TransitionState.Finished;//this is a hack bug fix. This will be more elegant in future updates!
            tickerTransition.TriggerTransition();//start the animation
        }

        // Function Stops the News Ticker
        public void StopBroadcast()
        {
            tickerTransition.Stop(false);
            StopAllCoroutines();
        }

        /// <summary>
        /// resumes the news ticker
        /// </summary>
        public void StartBroadcast()
        {
            tickerTransition.TriggerTransition();
        }
    }
}
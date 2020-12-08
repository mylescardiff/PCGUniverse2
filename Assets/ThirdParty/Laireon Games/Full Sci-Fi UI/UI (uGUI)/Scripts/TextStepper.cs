using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class TextStepper : MonoBehaviour
    {
        public Text theLabel;
        public float startValue = 0f;
        public enum ValueFormat { Normal, TimeFormat }
        public ValueFormat valueFormat = ValueFormat.Normal;
        private float finalValue;
        public float FinalValue
        {
            get
            {
                return finalValue;
            }
            set
            {
                finalValue = value;
            }
        }

        void Start()
        {
            theLabel.text = startValue.ToString();
        }


        IEnumerator NumberStepper(float stepDuration)
        {
            float t = 0.0f;
            float stepValue = 0f;
            while(t < stepDuration)
            {
                stepValue = Mathf.Lerp(0f, finalValue, Mathf.SmoothStep(0.0f, 1.0f, t / stepDuration));
                t += Time.deltaTime;
                switch(valueFormat)
                {
                    case ValueFormat.Normal:
                        theLabel.text = Mathf.RoundToInt(stepValue).ToString();

                        break;
                    case ValueFormat.TimeFormat:
                        int timeVal = (int)stepValue;
                        theLabel.text = OutputTime(timeVal);

                        break;
                }

                yield return null;
            }
            stepValue = finalValue;
        }

        // Function outputs the time in (hh:mm:ss) format
        string OutputTime(int _seconds)
        {
            // Calculate the hours
            int hours = _seconds / 3600;
            // Calculate the minutes
            int minutes = _seconds / 60;
            // Calculate the remaining seconds
            int seconds = _seconds % 100;
            // Produce string
            string output = hours.ToString("D2") + ":" + (minutes - (hours * 60)).ToString("D2") + ":" + seconds.ToString();
            // Return the string
            //		print (seconds);
            return output;
        }

        public void PlayStepper(float stepDuration)
        {
            StartCoroutine(NumberStepper(stepDuration));
        }

    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace LaireonFramework
{
    public class PieSegment : MonoBehaviour
    {

        // References
        public Image image;
        // Data
        float pieAmount;
        float fillPercentage = 0f;
        // Animation

        public float FillPercentage
        {
            set
            {
                fillPercentage = value;
            }
        }

        void Start()
        {
            image.fillAmount = 0f;
        }


        public void Reset()
        {
            image.fillAmount = 0;
        }

        IEnumerator SegmentFill(float fillDuration)
        {
            float t = 0.0f; pieAmount = 0f;
            while(t < fillDuration)
            {
                pieAmount = Mathf.Lerp(0f, fillPercentage, Mathf.SmoothStep(0.0f, 1.0f, t / fillDuration));
                t += Time.deltaTime;
                yield return null;
                image.fillAmount = pieAmount;
            }
            pieAmount = fillPercentage;

        }

        public void PlaySegment(float fillDuration = 1f)
        {
            StartCoroutine(SegmentFill(fillDuration));
        }
    }
}
